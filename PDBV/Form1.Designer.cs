namespace PDBV
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            // メインフォームの設定
            this.Text = "PDB Breakpoint Viewer";
            this.Size = new Size(800, 600);

            // パネルの作成（2列レイアウト用）
            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250
            };

            var rightPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // ファイル選択ボタン
            this.btnOpenPdb = new Button
            {
                Text = "PDBファイルを開く",
                Dock = DockStyle.Top,
                Height = 30
            };
            btnOpenPdb.Click += btnOpenPdb_Click;

            // ファイルツリービュー
            this.fileTreeView = new TreeView
            {
                Dock = DockStyle.Fill
            };
            fileTreeView.AfterSelect += FileTreeView_AfterSelect;

            // ブレークポイント情報表示用のリストビュー
            this.breakpointListView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true
            };

            // リストビューのカラム設定
            breakpointListView.Columns.Add("行", 60);
            breakpointListView.Columns.Add("開始列", 60);
            breakpointListView.Columns.Add("終了列", 60);
            breakpointListView.Columns.Add("IL Offset", 80);
            breakpointListView.Columns.Add("種類", 100);

            // パネルにコントロールを追加
            leftPanel.Controls.Add(fileTreeView);
            leftPanel.Controls.Add(btnOpenPdb);
            rightPanel.Controls.Add(breakpointListView);

            // フォームにパネルを追加
            this.Controls.Add(rightPanel);
            this.Controls.Add(leftPanel);

            this.openFileDialog = new OpenFileDialog
            {
                Filter = "PDB files (*.pdb)|*.pdb|All files (*.*)|*.*",
                Title = "PDBファイルを選択"
            };
        }

        private Button btnOpenPdb;
        private TreeView fileTreeView;
        private ListView breakpointListView;
        private OpenFileDialog openFileDialog;

        #endregion
    }
}
