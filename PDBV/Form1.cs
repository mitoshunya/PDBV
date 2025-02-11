using Microsoft.DiaSymReader;
using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace PDBV
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOpenPdb_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                LoadPdbFile(openFileDialog.FileName);
            }
        }

        private void FileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            breakpointListView.Items.Clear();
            if (e.Node.Tag is BreakpointInfo[] breakpoints)
            {
                foreach (var bp in breakpoints)
                {
                    var item = new ListViewItem(new[]
                    {
                        bp.StartLine.ToString(),
                        bp.StartColumn.ToString(),
                        bp.EndColumn.ToString(),
                        $"0x{bp.ILOffset:X}",
                        bp.IsHidden ? "Hidden" : "Normal"
                    });
                    breakpointListView.Items.Add(item);
                }
            }
        }

        private class BreakpointInfo
        {
            public int StartLine { get; set; }
            public int StartColumn { get; set; }
            public int EndColumn { get; set; }
            public int ILOffset { get; set; }
            public bool IsHidden { get; set; }
        }

        private void LoadPdbFile(string pdbPath)
        {
            try
            {
                fileTreeView.Nodes.Clear();
                breakpointListView.Items.Clear();

                using var fileStream = File.OpenRead(pdbPath);
                using var provider = MetadataReaderProvider.FromPortablePdbStream(fileStream);
                var reader = provider.GetMetadataReader();

                var rootNode = fileTreeView.Nodes.Add(Path.GetFileName(pdbPath));

                // �h�L�������g���ƂɃu���[�N�|�C���g�𐮗�
                foreach (var docHandle in reader.Documents)
                {
                    var document = reader.GetDocument(docHandle);
                    var documentName = reader.GetString(document.Name);
                    var fileNode = rootNode.Nodes.Add(Path.GetFileName(documentName));
                    var breakpoints = new List<BreakpointInfo>();

                    // ���̃h�L�������g�Ɋ֘A���邷�ׂẴu���[�N�|�C���g�����W
                    foreach (var methodDebugHandle in reader.MethodDebugInformation)
                    {
                        var debugInfo = reader.GetMethodDebugInformation(methodDebugHandle);
                        if (!debugInfo.SequencePointsBlob.IsNil)
                        {
                            foreach (var seqPoint in debugInfo.GetSequencePoints())
                            {
                                if (!seqPoint.Document.IsNil && seqPoint.Document == docHandle)
                                {
                                    breakpoints.Add(new BreakpointInfo
                                    {
                                        StartLine = seqPoint.StartLine,
                                        StartColumn = seqPoint.StartColumn,
                                        EndColumn = seqPoint.EndColumn,
                                        ILOffset = seqPoint.Offset,
                                        IsHidden = seqPoint.IsHidden
                                    });
                                }
                            }
                        }
                    }

                    // �u���[�N�|�C���g���s�ԍ��Ń\�[�g
                    breakpoints.Sort((a, b) => a.StartLine.CompareTo(b.StartLine));
                    fileNode.Tag = breakpoints.ToArray();

                    // �u���[�N�|�C���g����\��
                    fileNode.Text = $"{Path.GetFileName(documentName)} ({breakpoints.Count} breakpoints)";
                }

                rootNode.Expand();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PDB�t�@�C���̓ǂݍ��ݒ��ɃG���[���������܂����F\n{ex.Message}",
                              "�G���[", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // �V���v���ȃ��^�f�[�^�v���o�C�_�[����
    public class SymMetadataProvider : ISymReaderMetadataProvider
    {
        public bool TryGetStandaloneSignature(int standaloneSignatureToken, out byte[] signature)
        {
            signature = null;
            return false;
        }

        public unsafe bool TryGetStandaloneSignature(int standaloneSignatureToken, out byte* signature, out int length)
        {
            throw new NotImplementedException();
        }

        public bool TryGetTypeDefinitionInfo(int typeDefinitionToken, out string namespaceName,
            out string typeName, out TypeAttributes attributes)
        {
            namespaceName = null;
            typeName = null;
            attributes = 0;
            return false;
        }

        public bool TryGetTypeReferenceInfo(int typeReferenceToken, out string namespaceName,
            out string typeName)
        {
            namespaceName = null;
            typeName = null;
            return false;
        }
    }
}

