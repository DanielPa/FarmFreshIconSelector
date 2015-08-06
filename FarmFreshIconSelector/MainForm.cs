using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FarmFreshIconSelector
{
    public partial class MainForm : Form
    {
        /* Fields description:
         * _iconCount = number of enums/icons in the library
         * _smallImages = ImageList with all 16x16 sized images
         * _largeImages = ImageList with all 32x32 sized images
         */
        private int _iconCount;
        private ImageList _smallImages;
        private ImageList _largeImages;
        
        public MainForm()
        {            
            InitializeComponent();
        }
        
        private void populateListView()
        {
            toolStripStatusLabel1.Text = "Updating items..";
            this.Refresh();
            listViewIconsView.Items.Clear();
            for (int i = 1; i < _iconCount; i++)
            {
                listViewIconsView.Items.Add(getImageNameByIndex(i));
                listViewIconsView.Items[i - 1].Name = getImageNameByIndex(i);
                listViewIconsView.Items[i - 1].ImageIndex = i - 1;
                toolStripProgressBar1.PerformStep();
            }
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Ready";
        }

        private void setListviewItemImages()
        {
            toolStripStatusLabel1.Text = "Creating images..";
            this.Refresh();
            for (int i = 1; i < _iconCount; i++)
            {
                _smallImages.Images.Add(getImageNameByIndex(i), getBitmapByIndex(i, false));
                _largeImages.Images.Add(getImageNameByIndex(i), getBitmapByIndex(i, true));
                toolStripProgressBar1.PerformStep();
            }
            listViewIconsView.SmallImageList = _smallImages;
            listViewIconsView.LargeImageList = _largeImages;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabel1.Text = "Ready";
        }

        private Bitmap getBitmapByIndex(int imageIndex, bool largeImage)
        {
            Z.IconLibrary.FarmFresh.Icon ico = (Z.IconLibrary.FarmFresh.Icon)imageIndex;
            if (largeImage)
            {
                return ico.GetBitmap32();
            }
            else
            {
                return ico.GetBitmap16();
            }
        }

        private string getImageNameByIndex(int imageIndex)
        {
            Z.IconLibrary.FarmFresh.Icon ico = (Z.IconLibrary.FarmFresh.Icon)imageIndex;
            return ico.ToString();
        }

        private DataObject getSelectedItemFiles()
        {
            string tempPath = Path.GetTempPath();
            string[] tempFiles = new string[listViewIconsView.SelectedItems.Count];
            if (imageSize16x16pxToolStripMenuItem.Checked)
            {
                for (int i = 0; i < listViewIconsView.SelectedItems.Count; i++)
                {
                    int imageIndex = listViewIconsView.SmallImageList.Images.IndexOfKey(listViewIconsView.SelectedItems[i].Text);
                    tempFiles[i] = Path.Combine(tempPath, listViewIconsView.SelectedItems[i].Text + "_16.ico");
                    listViewIconsView.SmallImageList.Images[imageIndex].Save(tempFiles[i]);
                }
            }
            if (imageSize32x32pxToolStripMenuItem.Checked)
            {
                for (int i = 0; i < listViewIconsView.SelectedItems.Count; i++)
                {
                    int imageIndex = listViewIconsView.LargeImageList.Images.IndexOfKey(listViewIconsView.SelectedItems[i].Text);
                    tempFiles[i] = Path.Combine(tempPath, listViewIconsView.SelectedItems[i].Text + "_32.ico");
                    listViewIconsView.LargeImageList.Images[imageIndex].Save(tempFiles[i]);
                }
            }
            DataObject fileData = new DataObject();
            fileData.SetData(DataFormats.FileDrop, tempFiles);
            return fileData;
        }

        #region Event controlled methodes
        private void MainForm_Load(object sender, EventArgs e)
        {            
            _iconCount = Enum.GetValues(typeof(Z.IconLibrary.FarmFresh.Icon)).Length;
            _smallImages = new ImageList();
            _largeImages = new ImageList();
            //ImageSize has to be set because default is 16x16
            _smallImages.ImageSize = new Size(16, 16);
            _largeImages.ImageSize = new Size(32, 32);
            //MainForm icon
            this.Icon = Z.IconLibrary.FarmFresh.Icon.Images.GetIcon16();
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = _iconCount;
            toolStripProgressBar1.Step = 1;
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabelCount.Text = "";
            imageSize16x16pxToolStripMenuItem.Checked = true;
            imageSize32x32pxToolStripMenuItem.Checked = false;
            listViewIconsView.View = View.List;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            //separeted from Load event to show the window bevor populating listview
            //more user-friendly cause user gets feedback from progressbar
            this.Refresh();
            populateListView();
            setListviewItemImages();
            toolStripStatusLabelCount.Text = listViewIconsView.Items.Count.ToString() + " Images";

        }

        private void imageSize16x16pxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageSize32x32pxToolStripMenuItem.Checked = false;
            imageSize16x16pxToolStripMenuItem.Checked = true;
            listViewIconsView.View = View.List;
        }

        private void imageSize32x32pxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            imageSize16x16pxToolStripMenuItem.Checked = false;
            imageSize32x32pxToolStripMenuItem.Checked = true;
            listViewIconsView.View = View.LargeIcon;
        }
                
        private void listViewIconsView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            DoDragDrop(getSelectedItemFiles(), DragDropEffects.Copy);
        }
              
        private void listViewIconsView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            int count = listViewIconsView.SelectedItems.Count;
            switch (count)
            {
                case 0: toolStripStatusLabelSelection.Text = "";
                    break;
                case 1: toolStripStatusLabelSelection.Text = "1 Image selected";
                    break;
                default: toolStripStatusLabelSelection.Text = count.ToString() + " Images selected";
                    break;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog();
        }
     
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Searching...";
            this.Refresh();
            if (textBoxFilter.Text != "")
            {
                for (int i = listViewIconsView.Items.Count - 1; i >= 0; i--)
                {
                    var item = listViewIconsView.Items[i];
                    if (item.Text.ToLower().Contains(textBoxFilter.Text.ToLower()))
                    {

                    }
                    else
                    {
                        listViewIconsView.Items.Remove(item);
                    }
                    toolStripProgressBar1.PerformStep();
                }
                if (listViewIconsView.SelectedItems.Count == 1)
                {
                    listViewIconsView.Focus();
                }
            }
            else
                populateListView();
            toolStripProgressBar1.Value = 0;
            toolStripStatusLabelCount.Text = listViewIconsView.Items.Count.ToString() + " Images";
            toolStripStatusLabel1.Text = "Ready";
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBoxFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                buttonSearch_Click(sender, e);
            }
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listViewIconsView.Items)
            {
                item.Selected = true;
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetDataObject(getSelectedItemFiles());
        }

        private void saveToToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewIconsView.SelectedItems.Count > 0)
            {
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    string targetFolder = folderBrowserDialog.SelectedPath;
                    foreach (var fileName in getSelectedItemFiles().GetFileDropList())
                    {
                        File.Copy(fileName, Path.Combine(targetFolder, Path.GetFileName(fileName)));
                    }
                }
            }
            else
            {
                MessageBox.Show("Please first select images to save.", "Save To", MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                textBoxFilter.Focus();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
