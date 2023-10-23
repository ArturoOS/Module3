using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileSystem
{
	public class FileSystemVisitor
	{
		public delegate string Filter(string pathToFilter);
		public event EventHandler OnStart;
		public event EventHandler OnFinish;
		public event EventHandler OnFileFound;
		public event EventHandler OnDirectoryFound;
		public event EventHandler OnFilteredFileFound;
		public event EventHandler OnFilteredDirectoryFound;
		private bool FilterSearch = false;
		private Filter filterDelegate;

		public FileSystemVisitor() {}
		public FileSystemVisitor(Filter filterFunction) 
		{
			FilterSearch = true;
			filterDelegate = filterFunction;
		}

		public IEnumerable<string> SearchForFilesAndFolders(string path) 
		{
			OnStart?.Invoke(this, EventArgs.Empty);

			string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
			foreach (var file in files)
			{
				OnFileFound?.Invoke(this, EventArgs.Empty);
				if (!FilterSearch)
				{
					yield return file + "\n";
				}
				else 
				{
					if (!string.IsNullOrEmpty(filterDelegate(file)))
					{
						yield return file + "\n";
					}
					else 
					{
						OnFilteredFileFound?.Invoke(this, EventArgs.Empty);
						yield return "";
					}
				}
			}

			string[] directories = Directory.GetDirectories(path, "*.*", SearchOption.AllDirectories);
			foreach (var directory in directories)
			{
				OnDirectoryFound?.Invoke(this, EventArgs.Empty);
				if (!FilterSearch)
				{
					yield return directory + "\n";
				}
				else
				{
					if (!string.IsNullOrEmpty(filterDelegate(directory)))
					{
						yield return directory + "\n";
					}
					else 
					{
						OnFilteredDirectoryFound?.Invoke(this, EventArgs.Empty);
						yield return "";
					}
				}
			}

			OnFinish?.Invoke(this, EventArgs.Empty);
		}
	}
}
