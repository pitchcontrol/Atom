using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Atom.Interfaces;
using Atom.Models;

namespace Atom.Services
{
    public class DiskPath : IDiskPath
    {
        private readonly Dictionary<string, PathDialog> _dictionary = new Dictionary<string, PathDialog>();

        public DiskPath Add(string name, PathDialog path)
        {
            _dictionary.Add(name, path);
            return this;
        }

        public string this[string name] => _dictionary[name].Value;

        public string Path { get; set; }
        public bool GetPath(string name)
        {
            PathDialog dialog = _dictionary[name];
            if (dialog.Cache && !string.IsNullOrEmpty(dialog.Value))
            {
                Path = dialog.Value;
                return true;
            }
            if (dialog.IsFolder)
            {
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog { Description = dialog.Description, SelectedPath = ConfigurationManager.AppSettings[dialog.DefaultPathName] };
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    dialog.Value = folderBrowserDialog.SelectedPath;
                    Path = dialog.Value;
                    return true;
                }
            }
            else
            {
                if (dialog.OpenDialog)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog()
                    {
                        InitialDirectory = ConfigurationManager.AppSettings[dialog.DefaultPathName],
                        Filter = dialog.Filter,
                        Multiselect = false
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        dialog.Value = openFileDialog.FileName;
                        Path = dialog.Value;
                        return true;
                    }
                }
                else
                {
                    SaveFileDialog openFileDialog = new SaveFileDialog()
                    {
                        InitialDirectory = ConfigurationManager.AppSettings[dialog.DefaultPathName],
                        Filter = dialog.Filter
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        dialog.Value = openFileDialog.FileName;
                        Path = dialog.Value;
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
