﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication1.Forms;
using WindowsFormsApplication1;
using System.IO;

namespace WindowsFormsApplication1
{
    public partial class MainForm : Form
    {
        // Declaring private fields
        private Scene currentScene;
        private bool dragAllowed;
        private bool isLocked;

        // Constructor
        public MainForm()
        {
            // Initializes form and creates a new scene
            InitializeComponent();
            this.currentScene = new Scene(this);
            this.dragAllowed = true;
            this.isLocked = false;
            sceneNameLabel.Text = this.currentScene.getName();
            updateForm();
        }

        // Closes the program
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Shows the add panels window which allows you to add panels to the current scene
        private void addItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newWindow = new addPanelForm(this);
            newWindow.Show();
        }

        // Function that gets called from the add panels window to add a panel to the scene
        public void addBlockPanel(BlockPanel panel)
        {
            this.currentScene.addBlock(panel);
            updateForm();
        }

        // Shows the help form
        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newWindow = new HelpForm();
            newWindow.Show();
        }

        // Update the FlowLayout container with current scene's block array
        private void updateBlockContainer()
        {
            flowLayoutPanel.Controls.Clear();
            if (this.currentScene.getBlockArray().Count > 0)
            {
                this.noPanelsLabel.Hide();
                for (int x = 0; x < this.currentScene.getBlockArray().Count; x++)
                {
                    if (isLocked == false)
                    {
                        this.currentScene.getBlockArray()[x].startDemo();
                    }
                    else
                    {
                        this.currentScene.getBlockArray()[x].endDemo();
                    }

                    this.flowLayoutPanel.Controls.Add(this.currentScene.getBlockArray()[x]);
                }
            }
            else
            {
                this.noPanelsLabel.Show();
            }

        }

        // A public method to update the form after changes were made to it
        public void updateForm()
        {
            updateBlockContainer();
            sceneNameLabel.Text = this.currentScene.getName();
        }

        // Controls drag and drop inside the FlowLayout container
        private void flowLayoutPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (dragAllowed == true)
            {

            }
        }

        // Locks drag and drop as well as other scene menus
        private void lockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lockToolStripMenuItem.Text == "Lock")
            {
                addItemToolStripMenuItem.Enabled = false;
                removePanelToolStripMenuItem.Enabled = false;
                this.dragAllowed = false;
                this.isLocked = true;
                lockToolStripMenuItem.Text = "Unlock";
                statusLabel.Text = "ScoreBoardBlocksOBS is now active. Blocks are now able to be interacted with and output files will begin outputting.";
                this.currentScene.activateBlocks();
            } 
            else if (lockToolStripMenuItem.Text == "Unlock")
            {
                addItemToolStripMenuItem.Enabled = true;
                removePanelToolStripMenuItem.Enabled = true;
                this.dragAllowed = true;
                this.isLocked = false;
                lockToolStripMenuItem.Text = "Lock";
                statusLabel.Text = "ScoreBoardBlocksOBS is currently in designer mode. Click Edit Current Scene > Lock in order to interact with blocks.";
            }
            updateBlockContainer();
        }

        // Opens the remove panel window
        private void removePanelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newWindow = new RemovePanelForm(this);
            newWindow.Show();
        }

        // Public method to obtain the current scene from anywhere
        public Scene getCurrentScene()
        {
            return this.currentScene;
        }

        // Opens the settings page for the current scene
        private void sceneSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form newWindow = new SceneSettingsForm(this);
            newWindow.Show();
        }

        // Asks the user before wiping the scene
        private void newSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialog = MessageBox.Show("Are you sure you want to create a new scene? Any unsaved changes will be lost.", "", MessageBoxButtons.YesNo);
            if (dialog == DialogResult.Yes)
            {
                this.currentScene = new Scene(this);
                updateForm();
            }
        }

        // Loads the instance of the current scene from a file and loads it into the current session
        private void loadSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Code to initialize file dialog
            openFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            openFileDialog1.Filter = "Scene Files (*.sbb) |*.sbb";
            openFileDialog1.FileName = "";
            DialogResult saveDialog = openFileDialog1.ShowDialog();

            if (saveDialog == DialogResult.OK || !saveFileDialog1.CheckPathExists)
            {
                // Deserializes from file
                using (Stream stream = File.Open(openFileDialog1.FileName, FileMode.Open))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                    this.currentScene = (Scene)bformatter.Deserialize(stream);
                    // Since we had to convert all BlockPanels to string equilivents, now we have to load those
                    // strings and convert them into actual blocks
                    this.currentScene.prepareForLoad();
                    updateForm();

                    MessageBox.Show("Scene loaded successfully.");
                }
            }
        }

        // Saves the instance of the current scene to a file
        private void saveSceneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Code to initialize file dialog
            saveFileDialog1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            saveFileDialog1.FileName = this.currentScene.getName() + ".sbb";
            saveFileDialog1.Filter = "Scene Files (*.sbb) |*.sbb";
            DialogResult saveDialog = saveFileDialog1.ShowDialog();

            if (saveDialog == DialogResult.OK || !saveFileDialog1.CheckPathExists) 
            {
                // Serialize the Scene object to file
                using (Stream stream = File.Open(saveFileDialog1.FileName, FileMode.Create))
                {
                    var bformatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    // Since usercontrols are not serializable, the function below converts all the unserializable
                    // BlockPanels into their string equivilents, which allow the scene to be serialized
                    this.currentScene.prepareForSave();
                    bformatter.Serialize(stream, this.currentScene);

                    MessageBox.Show("Scene saved successfully.");
                }
            }
        }
    }
}
