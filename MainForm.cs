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
        private int _iconCount;
        private ImageList _smallImages;
        private ImageList _largeImages;
        
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _iconCount = Enum.GetValues(typeof(Z.IconLibrary.FarmFresh.Icon)).Length;
            _smallImages = new ImageList();
            _largeImages = new ImageList();
            _smallImages.ImageSize = new Size(16, 16);
            _largeImages.ImageSize = new Size(32, 32);
            this.Icon = Z.IconLibrary.FarmFresh.Icon.Images.GetIcon16();
            toolStripProgressBar1.Minimum = 0;
            toolStripProgressBar1.Maximum = _iconCount;
            toolStripProgressBar1.Step = 1;
            toolStripStatusLabelCount.Text = _iconCount.ToString() + " Images";
            imageSize16x16pxToolStripMenuItem.Checked = true;
            imageSize32x32pxToolStripMenuItem.Checked = false;
            listViewIconsView.View = View.List;            
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            this.Refresh();
            PopulateListView();
            toolStripStatusLabel1.Text = "Ready";
            toolStripProgressBar1.Value = 0;
        }

        private void PopulateListView()
        {
            for (int i = 1; i < _iconCount; i++)
            {
                _smallImages.Images.Add(getImageNameByIndex(i), getBitmapByIndex(i, false));
                _largeImages.Images.Add(getImageNameByIndex(i), getBitmapByIndex(i, true));
                listViewIconsView.Items.Add(getImageNameByIndex(i));
                listViewIconsView.Items[i - 1].ImageIndex = i - 1;
                toolStripProgressBar1.PerformStep();
            }
            listViewIconsView.SmallImageList = _smallImages;
            listViewIconsView.LargeImageList = _largeImages;
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
            DoDragDrop(fileData, DragDropEffects.Copy);
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
            MessageBox.Show("Sorry, this is not yet implemented!");
        }
    }
}
