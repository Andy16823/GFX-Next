namespace NewGFXEditor
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            menuStrip1 = new MenuStrip();
            dateiToolStripMenuItem = new ToolStripMenuItem();
            neuToolStripMenuItem = new ToolStripMenuItem();
            öffnenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator = new ToolStripSeparator();
            speichernToolStripMenuItem = new ToolStripMenuItem();
            speichernunterToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            druckenToolStripMenuItem = new ToolStripMenuItem();
            seitenansichtToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            beendenToolStripMenuItem = new ToolStripMenuItem();
            bearbeitenToolStripMenuItem = new ToolStripMenuItem();
            rückgängigToolStripMenuItem = new ToolStripMenuItem();
            wiederholenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            ausschneidenToolStripMenuItem = new ToolStripMenuItem();
            kopierenToolStripMenuItem = new ToolStripMenuItem();
            einfügenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            allesauswählenToolStripMenuItem = new ToolStripMenuItem();
            selectionToolStripMenuItem = new ToolStripMenuItem();
            editPositionToolStripMenuItem = new ToolStripMenuItem();
            editRotationToolStripMenuItem = new ToolStripMenuItem();
            editScaleToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator11 = new ToolStripSeparator();
            deleteToolStripMenuItem = new ToolStripMenuItem();
            viewToolStripMenuItem = new ToolStripMenuItem();
            showAABBsToolStripMenuItem = new ToolStripMenuItem();
            createToolStripMenuItem = new ToolStripMenuItem();
            cubeToolStripMenuItem = new ToolStripMenuItem();
            sphereToolStripMenuItem = new ToolStripMenuItem();
            quadToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator10 = new ToolStripSeparator();
            modelToolStripMenuItem = new ToolStripMenuItem();
            extrasToolStripMenuItem = new ToolStripMenuItem();
            materialEditorToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            importMaterialToolStripMenuItem = new ToolStripMenuItem();
            assignSelectedMaterialToolStripMenuItem = new ToolStripMenuItem();
            hilfeToolStripMenuItem = new ToolStripMenuItem();
            inhaltToolStripMenuItem = new ToolStripMenuItem();
            indexToolStripMenuItem = new ToolStripMenuItem();
            suchenToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            infoToolStripMenuItem = new ToolStripMenuItem();
            toolStrip1 = new ToolStrip();
            neuToolStripButton = new ToolStripButton();
            öffnenToolStripButton = new ToolStripButton();
            speichernToolStripButton = new ToolStripButton();
            druckenToolStripButton = new ToolStripButton();
            toolStripSeparator6 = new ToolStripSeparator();
            ausschneidenToolStripButton = new ToolStripButton();
            kopierenToolStripButton = new ToolStripButton();
            einfügenToolStripButton = new ToolStripButton();
            toolStripSeparator7 = new ToolStripSeparator();
            hilfeToolStripButton = new ToolStripButton();
            layerComboBox = new ToolStripComboBox();
            toolStripSeparator9 = new ToolStripSeparator();
            gizmoModeTranslateBtn = new ToolStripButton();
            gizmoModeScaleBtn = new ToolStripButton();
            toolStripButton3 = new ToolStripButton();
            statusStrip1 = new StatusStrip();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            treeView1 = new TreeView();
            tabPage2 = new TabPage();
            materialListView = new ListView();
            materialImageList = new ImageList(components);
            tabPage3 = new TabPage();
            propertyGrid1 = new PropertyGrid();
            editSelectedMaterialToolStripMenuItem = new ToolStripMenuItem();
            menuStrip1.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer2).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { dateiToolStripMenuItem, bearbeitenToolStripMenuItem, selectionToolStripMenuItem, viewToolStripMenuItem, createToolStripMenuItem, extrasToolStripMenuItem, hilfeToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1247, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // dateiToolStripMenuItem
            // 
            dateiToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { neuToolStripMenuItem, öffnenToolStripMenuItem, toolStripSeparator, speichernToolStripMenuItem, speichernunterToolStripMenuItem, toolStripSeparator1, druckenToolStripMenuItem, seitenansichtToolStripMenuItem, toolStripSeparator2, beendenToolStripMenuItem });
            dateiToolStripMenuItem.Name = "dateiToolStripMenuItem";
            dateiToolStripMenuItem.Size = new Size(46, 20);
            dateiToolStripMenuItem.Text = "&Datei";
            // 
            // neuToolStripMenuItem
            // 
            neuToolStripMenuItem.Image = (Image)resources.GetObject("neuToolStripMenuItem.Image");
            neuToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            neuToolStripMenuItem.Name = "neuToolStripMenuItem";
            neuToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            neuToolStripMenuItem.Size = new Size(168, 22);
            neuToolStripMenuItem.Text = "&Neu";
            // 
            // öffnenToolStripMenuItem
            // 
            öffnenToolStripMenuItem.Image = (Image)resources.GetObject("öffnenToolStripMenuItem.Image");
            öffnenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            öffnenToolStripMenuItem.Name = "öffnenToolStripMenuItem";
            öffnenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            öffnenToolStripMenuItem.Size = new Size(168, 22);
            öffnenToolStripMenuItem.Text = "Ö&ffnen";
            // 
            // toolStripSeparator
            // 
            toolStripSeparator.Name = "toolStripSeparator";
            toolStripSeparator.Size = new Size(165, 6);
            // 
            // speichernToolStripMenuItem
            // 
            speichernToolStripMenuItem.Image = (Image)resources.GetObject("speichernToolStripMenuItem.Image");
            speichernToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            speichernToolStripMenuItem.Name = "speichernToolStripMenuItem";
            speichernToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            speichernToolStripMenuItem.Size = new Size(168, 22);
            speichernToolStripMenuItem.Text = "&Speichern";
            // 
            // speichernunterToolStripMenuItem
            // 
            speichernunterToolStripMenuItem.Name = "speichernunterToolStripMenuItem";
            speichernunterToolStripMenuItem.Size = new Size(168, 22);
            speichernunterToolStripMenuItem.Text = "Speichern &unter";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(165, 6);
            // 
            // druckenToolStripMenuItem
            // 
            druckenToolStripMenuItem.Image = (Image)resources.GetObject("druckenToolStripMenuItem.Image");
            druckenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            druckenToolStripMenuItem.Name = "druckenToolStripMenuItem";
            druckenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            druckenToolStripMenuItem.Size = new Size(168, 22);
            druckenToolStripMenuItem.Text = "&Drucken";
            // 
            // seitenansichtToolStripMenuItem
            // 
            seitenansichtToolStripMenuItem.Image = (Image)resources.GetObject("seitenansichtToolStripMenuItem.Image");
            seitenansichtToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            seitenansichtToolStripMenuItem.Name = "seitenansichtToolStripMenuItem";
            seitenansichtToolStripMenuItem.Size = new Size(168, 22);
            seitenansichtToolStripMenuItem.Text = "&Seitenansicht";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(165, 6);
            // 
            // beendenToolStripMenuItem
            // 
            beendenToolStripMenuItem.Name = "beendenToolStripMenuItem";
            beendenToolStripMenuItem.Size = new Size(168, 22);
            beendenToolStripMenuItem.Text = "&Beenden";
            // 
            // bearbeitenToolStripMenuItem
            // 
            bearbeitenToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { rückgängigToolStripMenuItem, wiederholenToolStripMenuItem, toolStripSeparator3, ausschneidenToolStripMenuItem, kopierenToolStripMenuItem, einfügenToolStripMenuItem, toolStripSeparator4, allesauswählenToolStripMenuItem });
            bearbeitenToolStripMenuItem.Name = "bearbeitenToolStripMenuItem";
            bearbeitenToolStripMenuItem.Size = new Size(75, 20);
            bearbeitenToolStripMenuItem.Text = "&Bearbeiten";
            // 
            // rückgängigToolStripMenuItem
            // 
            rückgängigToolStripMenuItem.Name = "rückgängigToolStripMenuItem";
            rückgängigToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            rückgängigToolStripMenuItem.Size = new Size(191, 22);
            rückgängigToolStripMenuItem.Text = "&Rückgängig";
            // 
            // wiederholenToolStripMenuItem
            // 
            wiederholenToolStripMenuItem.Name = "wiederholenToolStripMenuItem";
            wiederholenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            wiederholenToolStripMenuItem.Size = new Size(191, 22);
            wiederholenToolStripMenuItem.Text = "&Wiederholen";
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(188, 6);
            // 
            // ausschneidenToolStripMenuItem
            // 
            ausschneidenToolStripMenuItem.Image = (Image)resources.GetObject("ausschneidenToolStripMenuItem.Image");
            ausschneidenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            ausschneidenToolStripMenuItem.Name = "ausschneidenToolStripMenuItem";
            ausschneidenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.X;
            ausschneidenToolStripMenuItem.Size = new Size(191, 22);
            ausschneidenToolStripMenuItem.Text = "Aussc&hneiden";
            // 
            // kopierenToolStripMenuItem
            // 
            kopierenToolStripMenuItem.Image = (Image)resources.GetObject("kopierenToolStripMenuItem.Image");
            kopierenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            kopierenToolStripMenuItem.Name = "kopierenToolStripMenuItem";
            kopierenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.C;
            kopierenToolStripMenuItem.Size = new Size(191, 22);
            kopierenToolStripMenuItem.Text = "&Kopieren";
            // 
            // einfügenToolStripMenuItem
            // 
            einfügenToolStripMenuItem.Image = (Image)resources.GetObject("einfügenToolStripMenuItem.Image");
            einfügenToolStripMenuItem.ImageTransparentColor = Color.Magenta;
            einfügenToolStripMenuItem.Name = "einfügenToolStripMenuItem";
            einfügenToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.V;
            einfügenToolStripMenuItem.Size = new Size(191, 22);
            einfügenToolStripMenuItem.Text = "&Einfügen";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(188, 6);
            // 
            // allesauswählenToolStripMenuItem
            // 
            allesauswählenToolStripMenuItem.Name = "allesauswählenToolStripMenuItem";
            allesauswählenToolStripMenuItem.Size = new Size(191, 22);
            allesauswählenToolStripMenuItem.Text = "&Alles auswählen";
            // 
            // selectionToolStripMenuItem
            // 
            selectionToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { editPositionToolStripMenuItem, editRotationToolStripMenuItem, editScaleToolStripMenuItem, toolStripSeparator11, deleteToolStripMenuItem });
            selectionToolStripMenuItem.Name = "selectionToolStripMenuItem";
            selectionToolStripMenuItem.Size = new Size(67, 20);
            selectionToolStripMenuItem.Text = "Selection";
            // 
            // editPositionToolStripMenuItem
            // 
            editPositionToolStripMenuItem.Name = "editPositionToolStripMenuItem";
            editPositionToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.T;
            editPositionToolStripMenuItem.Size = new Size(185, 22);
            editPositionToolStripMenuItem.Text = "Edit Position";
            editPositionToolStripMenuItem.Click += editPositionToolStripMenuItem_Click;
            // 
            // editRotationToolStripMenuItem
            // 
            editRotationToolStripMenuItem.Name = "editRotationToolStripMenuItem";
            editRotationToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.R;
            editRotationToolStripMenuItem.Size = new Size(185, 22);
            editRotationToolStripMenuItem.Text = "Edit Rotation";
            editRotationToolStripMenuItem.Click += editRotationToolStripMenuItem_Click;
            // 
            // editScaleToolStripMenuItem
            // 
            editScaleToolStripMenuItem.Name = "editScaleToolStripMenuItem";
            editScaleToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            editScaleToolStripMenuItem.Size = new Size(185, 22);
            editScaleToolStripMenuItem.Text = "Edit Scale";
            editScaleToolStripMenuItem.Click += editScaleToolStripMenuItem_Click;
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new Size(182, 6);
            // 
            // deleteToolStripMenuItem
            // 
            deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            deleteToolStripMenuItem.Size = new Size(185, 22);
            deleteToolStripMenuItem.Text = "Delete";
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showAABBsToolStripMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new Size(44, 20);
            viewToolStripMenuItem.Text = "View";
            // 
            // showAABBsToolStripMenuItem
            // 
            showAABBsToolStripMenuItem.CheckOnClick = true;
            showAABBsToolStripMenuItem.Name = "showAABBsToolStripMenuItem";
            showAABBsToolStripMenuItem.Size = new Size(144, 22);
            showAABBsToolStripMenuItem.Text = "Show AABB's";
            showAABBsToolStripMenuItem.Click += showAABBsToolStripMenuItem_Click;
            // 
            // createToolStripMenuItem
            // 
            createToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cubeToolStripMenuItem, sphereToolStripMenuItem, quadToolStripMenuItem, toolStripSeparator10, modelToolStripMenuItem });
            createToolStripMenuItem.Name = "createToolStripMenuItem";
            createToolStripMenuItem.Size = new Size(72, 20);
            createToolStripMenuItem.Text = "Add Asset";
            createToolStripMenuItem.Click += createToolStripMenuItem_Click;
            // 
            // cubeToolStripMenuItem
            // 
            cubeToolStripMenuItem.Name = "cubeToolStripMenuItem";
            cubeToolStripMenuItem.Size = new Size(110, 22);
            cubeToolStripMenuItem.Text = "Cube";
            cubeToolStripMenuItem.Click += cubeToolStripMenuItem_Click;
            // 
            // sphereToolStripMenuItem
            // 
            sphereToolStripMenuItem.Name = "sphereToolStripMenuItem";
            sphereToolStripMenuItem.Size = new Size(110, 22);
            sphereToolStripMenuItem.Text = "Sphere";
            sphereToolStripMenuItem.Click += sphereToolStripMenuItem_Click;
            // 
            // quadToolStripMenuItem
            // 
            quadToolStripMenuItem.Name = "quadToolStripMenuItem";
            quadToolStripMenuItem.Size = new Size(110, 22);
            quadToolStripMenuItem.Text = "Quad";
            quadToolStripMenuItem.Click += quadToolStripMenuItem_Click;
            // 
            // toolStripSeparator10
            // 
            toolStripSeparator10.Name = "toolStripSeparator10";
            toolStripSeparator10.Size = new Size(107, 6);
            // 
            // modelToolStripMenuItem
            // 
            modelToolStripMenuItem.Name = "modelToolStripMenuItem";
            modelToolStripMenuItem.Size = new Size(110, 22);
            modelToolStripMenuItem.Text = "Model";
            modelToolStripMenuItem.Click += modelToolStripMenuItem_Click;
            // 
            // extrasToolStripMenuItem
            // 
            extrasToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { materialEditorToolStripMenuItem, toolStripSeparator8, importMaterialToolStripMenuItem, assignSelectedMaterialToolStripMenuItem, editSelectedMaterialToolStripMenuItem });
            extrasToolStripMenuItem.Name = "extrasToolStripMenuItem";
            extrasToolStripMenuItem.Size = new Size(67, 20);
            extrasToolStripMenuItem.Text = "Materials";
            // 
            // materialEditorToolStripMenuItem
            // 
            materialEditorToolStripMenuItem.Name = "materialEditorToolStripMenuItem";
            materialEditorToolStripMenuItem.Size = new Size(202, 22);
            materialEditorToolStripMenuItem.Text = "Create Material";
            materialEditorToolStripMenuItem.Click += materialEditorToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(199, 6);
            // 
            // importMaterialToolStripMenuItem
            // 
            importMaterialToolStripMenuItem.Name = "importMaterialToolStripMenuItem";
            importMaterialToolStripMenuItem.Size = new Size(202, 22);
            importMaterialToolStripMenuItem.Text = "Import Material";
            importMaterialToolStripMenuItem.Click += importMaterialToolStripMenuItem_Click;
            // 
            // assignSelectedMaterialToolStripMenuItem
            // 
            assignSelectedMaterialToolStripMenuItem.Name = "assignSelectedMaterialToolStripMenuItem";
            assignSelectedMaterialToolStripMenuItem.Size = new Size(202, 22);
            assignSelectedMaterialToolStripMenuItem.Text = "Assign Selected Material";
            assignSelectedMaterialToolStripMenuItem.Click += assignSelectedMaterialToolStripMenuItem_Click;
            // 
            // hilfeToolStripMenuItem
            // 
            hilfeToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { inhaltToolStripMenuItem, indexToolStripMenuItem, suchenToolStripMenuItem, toolStripSeparator5, infoToolStripMenuItem });
            hilfeToolStripMenuItem.Name = "hilfeToolStripMenuItem";
            hilfeToolStripMenuItem.Size = new Size(44, 20);
            hilfeToolStripMenuItem.Text = "&Hilfe";
            // 
            // inhaltToolStripMenuItem
            // 
            inhaltToolStripMenuItem.Name = "inhaltToolStripMenuItem";
            inhaltToolStripMenuItem.Size = new Size(113, 22);
            inhaltToolStripMenuItem.Text = "&Inhalt";
            // 
            // indexToolStripMenuItem
            // 
            indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            indexToolStripMenuItem.Size = new Size(113, 22);
            indexToolStripMenuItem.Text = "&Index";
            // 
            // suchenToolStripMenuItem
            // 
            suchenToolStripMenuItem.Name = "suchenToolStripMenuItem";
            suchenToolStripMenuItem.Size = new Size(113, 22);
            suchenToolStripMenuItem.Text = "&Suchen";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(110, 6);
            // 
            // infoToolStripMenuItem
            // 
            infoToolStripMenuItem.Name = "infoToolStripMenuItem";
            infoToolStripMenuItem.Size = new Size(113, 22);
            infoToolStripMenuItem.Text = "Inf&o...";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { neuToolStripButton, öffnenToolStripButton, speichernToolStripButton, druckenToolStripButton, toolStripSeparator6, ausschneidenToolStripButton, kopierenToolStripButton, einfügenToolStripButton, toolStripSeparator7, hilfeToolStripButton, layerComboBox, toolStripSeparator9, gizmoModeTranslateBtn, gizmoModeScaleBtn, toolStripButton3 });
            toolStrip1.Location = new Point(0, 24);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(1247, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // neuToolStripButton
            // 
            neuToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            neuToolStripButton.Image = (Image)resources.GetObject("neuToolStripButton.Image");
            neuToolStripButton.ImageTransparentColor = Color.Magenta;
            neuToolStripButton.Name = "neuToolStripButton";
            neuToolStripButton.Size = new Size(23, 22);
            neuToolStripButton.Text = "&Neu";
            // 
            // öffnenToolStripButton
            // 
            öffnenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            öffnenToolStripButton.Image = (Image)resources.GetObject("öffnenToolStripButton.Image");
            öffnenToolStripButton.ImageTransparentColor = Color.Magenta;
            öffnenToolStripButton.Name = "öffnenToolStripButton";
            öffnenToolStripButton.Size = new Size(23, 22);
            öffnenToolStripButton.Text = "Ö&ffnen";
            // 
            // speichernToolStripButton
            // 
            speichernToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            speichernToolStripButton.Image = (Image)resources.GetObject("speichernToolStripButton.Image");
            speichernToolStripButton.ImageTransparentColor = Color.Magenta;
            speichernToolStripButton.Name = "speichernToolStripButton";
            speichernToolStripButton.Size = new Size(23, 22);
            speichernToolStripButton.Text = "&Speichern";
            // 
            // druckenToolStripButton
            // 
            druckenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            druckenToolStripButton.Image = (Image)resources.GetObject("druckenToolStripButton.Image");
            druckenToolStripButton.ImageTransparentColor = Color.Magenta;
            druckenToolStripButton.Name = "druckenToolStripButton";
            druckenToolStripButton.Size = new Size(23, 22);
            druckenToolStripButton.Text = "&Drucken";
            // 
            // toolStripSeparator6
            // 
            toolStripSeparator6.Name = "toolStripSeparator6";
            toolStripSeparator6.Size = new Size(6, 25);
            // 
            // ausschneidenToolStripButton
            // 
            ausschneidenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            ausschneidenToolStripButton.Image = (Image)resources.GetObject("ausschneidenToolStripButton.Image");
            ausschneidenToolStripButton.ImageTransparentColor = Color.Magenta;
            ausschneidenToolStripButton.Name = "ausschneidenToolStripButton";
            ausschneidenToolStripButton.Size = new Size(23, 22);
            ausschneidenToolStripButton.Text = "&Ausschneiden";
            // 
            // kopierenToolStripButton
            // 
            kopierenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            kopierenToolStripButton.Image = (Image)resources.GetObject("kopierenToolStripButton.Image");
            kopierenToolStripButton.ImageTransparentColor = Color.Magenta;
            kopierenToolStripButton.Name = "kopierenToolStripButton";
            kopierenToolStripButton.Size = new Size(23, 22);
            kopierenToolStripButton.Text = "&Kopieren";
            // 
            // einfügenToolStripButton
            // 
            einfügenToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            einfügenToolStripButton.Image = (Image)resources.GetObject("einfügenToolStripButton.Image");
            einfügenToolStripButton.ImageTransparentColor = Color.Magenta;
            einfügenToolStripButton.Name = "einfügenToolStripButton";
            einfügenToolStripButton.Size = new Size(23, 22);
            einfügenToolStripButton.Text = "&Einfügen";
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(6, 25);
            // 
            // hilfeToolStripButton
            // 
            hilfeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            hilfeToolStripButton.Image = (Image)resources.GetObject("hilfeToolStripButton.Image");
            hilfeToolStripButton.ImageTransparentColor = Color.Magenta;
            hilfeToolStripButton.Name = "hilfeToolStripButton";
            hilfeToolStripButton.Size = new Size(23, 22);
            hilfeToolStripButton.Text = "Hi&lfe";
            // 
            // layerComboBox
            // 
            layerComboBox.Alignment = ToolStripItemAlignment.Right;
            layerComboBox.Name = "layerComboBox";
            layerComboBox.Size = new Size(121, 25);
            layerComboBox.SelectedIndexChanged += layerComboBox_SelectedIndexChanged;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(6, 25);
            // 
            // gizmoModeTranslateBtn
            // 
            gizmoModeTranslateBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            gizmoModeTranslateBtn.Image = (Image)resources.GetObject("gizmoModeTranslateBtn.Image");
            gizmoModeTranslateBtn.ImageTransparentColor = Color.Magenta;
            gizmoModeTranslateBtn.Name = "gizmoModeTranslateBtn";
            gizmoModeTranslateBtn.Size = new Size(23, 22);
            gizmoModeTranslateBtn.Text = "toolStripButton1";
            gizmoModeTranslateBtn.Click += gizmoModeTranslateBtn_Click;
            // 
            // gizmoModeScaleBtn
            // 
            gizmoModeScaleBtn.DisplayStyle = ToolStripItemDisplayStyle.Image;
            gizmoModeScaleBtn.Image = (Image)resources.GetObject("gizmoModeScaleBtn.Image");
            gizmoModeScaleBtn.ImageTransparentColor = Color.Magenta;
            gizmoModeScaleBtn.Name = "gizmoModeScaleBtn";
            gizmoModeScaleBtn.Size = new Size(23, 22);
            gizmoModeScaleBtn.Text = "toolStripButton2";
            gizmoModeScaleBtn.Click += gizmoModeScaleBtn_Click;
            // 
            // toolStripButton3
            // 
            toolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton3.Image = (Image)resources.GetObject("toolStripButton3.Image");
            toolStripButton3.ImageTransparentColor = Color.Magenta;
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(23, 22);
            toolStripButton3.Text = "toolStripButton3";
            // 
            // statusStrip1
            // 
            statusStrip1.Location = new Point(0, 675);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1247, 22);
            statusStrip1.TabIndex = 2;
            statusStrip1.Text = "statusStrip1";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 49);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(1247, 626);
            splitContainer1.SplitterDistance = 286;
            splitContainer1.TabIndex = 3;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(tabControl1);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(propertyGrid1);
            splitContainer2.Size = new Size(286, 626);
            splitContainer2.SplitterDistance = 409;
            splitContainer2.TabIndex = 0;
            // 
            // tabControl1
            // 
            tabControl1.Alignment = TabAlignment.Left;
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Multiline = true;
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(286, 409);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(treeView1);
            tabPage1.Location = new Point(27, 4);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(255, 401);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Scene";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // treeView1
            // 
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(3, 3);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(249, 395);
            treeView1.TabIndex = 0;
            treeView1.AfterSelect += treeView1_AfterSelect;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(materialListView);
            tabPage2.Location = new Point(27, 4);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(255, 401);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Materials";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // materialListView
            // 
            materialListView.AllowDrop = true;
            materialListView.Dock = DockStyle.Fill;
            materialListView.LargeImageList = materialImageList;
            materialListView.Location = new Point(3, 3);
            materialListView.Name = "materialListView";
            materialListView.Size = new Size(249, 395);
            materialListView.TabIndex = 0;
            materialListView.UseCompatibleStateImageBehavior = false;
            materialListView.DragDrop += materialListView_DragDrop;
            materialListView.DragEnter += materialListView_DragEnter;
            materialListView.DoubleClick += materialListView_DoubleClick;
            // 
            // materialImageList
            // 
            materialImageList.ColorDepth = ColorDepth.Depth32Bit;
            materialImageList.ImageSize = new Size(64, 64);
            materialImageList.TransparentColor = Color.Transparent;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(27, 4);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(255, 401);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Models";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = DockStyle.Fill;
            propertyGrid1.Location = new Point(0, 0);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.Size = new Size(286, 213);
            propertyGrid1.TabIndex = 0;
            // 
            // editSelectedMaterialToolStripMenuItem
            // 
            editSelectedMaterialToolStripMenuItem.Name = "editSelectedMaterialToolStripMenuItem";
            editSelectedMaterialToolStripMenuItem.Size = new Size(202, 22);
            editSelectedMaterialToolStripMenuItem.Text = "Edit Selected Material";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1247, 697);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "GFX 3D World Editor";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer2).EndInit();
            splitContainer2.ResumeLayout(false);
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem dateiToolStripMenuItem;
        private ToolStripMenuItem neuToolStripMenuItem;
        private ToolStripMenuItem öffnenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator;
        private ToolStripMenuItem speichernToolStripMenuItem;
        private ToolStripMenuItem speichernunterToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem druckenToolStripMenuItem;
        private ToolStripMenuItem seitenansichtToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem beendenToolStripMenuItem;
        private ToolStripMenuItem bearbeitenToolStripMenuItem;
        private ToolStripMenuItem rückgängigToolStripMenuItem;
        private ToolStripMenuItem wiederholenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem ausschneidenToolStripMenuItem;
        private ToolStripMenuItem kopierenToolStripMenuItem;
        private ToolStripMenuItem einfügenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem allesauswählenToolStripMenuItem;
        private ToolStripMenuItem extrasToolStripMenuItem;
        private ToolStripMenuItem hilfeToolStripMenuItem;
        private ToolStripMenuItem inhaltToolStripMenuItem;
        private ToolStripMenuItem indexToolStripMenuItem;
        private ToolStripMenuItem suchenToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private ToolStripMenuItem infoToolStripMenuItem;
        private ToolStrip toolStrip1;
        private ToolStripButton neuToolStripButton;
        private ToolStripButton öffnenToolStripButton;
        private ToolStripButton speichernToolStripButton;
        private ToolStripButton druckenToolStripButton;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripButton ausschneidenToolStripButton;
        private ToolStripButton kopierenToolStripButton;
        private ToolStripButton einfügenToolStripButton;
        private ToolStripSeparator toolStripSeparator7;
        private ToolStripButton hilfeToolStripButton;
        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem materialEditorToolStripMenuItem;
        private SplitContainer splitContainer2;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TreeView treeView1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private PropertyGrid propertyGrid1;
        private ToolStripMenuItem importMaterialToolStripMenuItem;
        private ListView materialListView;
        private ImageList materialImageList;
        private ToolStripMenuItem assignSelectedMaterialToolStripMenuItem;
        private ToolStripMenuItem createToolStripMenuItem;
        private ToolStripMenuItem cubeToolStripMenuItem;
        private ToolStripMenuItem sphereToolStripMenuItem;
        private ToolStripMenuItem quadToolStripMenuItem;
        private ToolStripComboBox layerComboBox;
        private ToolStripMenuItem selectionToolStripMenuItem;
        private ToolStripMenuItem editPositionToolStripMenuItem;
        private ToolStripMenuItem editRotationToolStripMenuItem;
        private ToolStripMenuItem editScaleToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripButton gizmoModeTranslateBtn;
        private ToolStripButton gizmoModeScaleBtn;
        private ToolStripButton toolStripButton3;
        private ToolStripSeparator toolStripSeparator10;
        private ToolStripMenuItem modelToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator11;
        private ToolStripMenuItem deleteToolStripMenuItem;
        private ToolStripMenuItem viewToolStripMenuItem;
        private ToolStripMenuItem showAABBsToolStripMenuItem;
        private ToolStripMenuItem editSelectedMaterialToolStripMenuItem;
    }
}
