using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FileSystem
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public event EventHandler<bool> OnCancel;
		public bool IsCancel;
		public MainWindow()
		{
			InitializeComponent();
		}

		private async void Button_Click(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog openFileDlg = new FolderBrowserDialog();
			openFileDlg.ShowDialog();

			if (!string.IsNullOrEmpty(openFileDlg.SelectedPath))
			{
				DirectoryBox.Text = openFileDlg.SelectedPath;
				
				FileSystemVisitor fileSystemVisitor;
				if (!string.IsNullOrWhiteSpace(FilterBox.Text))
				{
					string nameToExclude = FilterBox.Text;
					fileSystemVisitor = new FileSystemVisitor(file => file.Contains(nameToExclude) ? string.Empty : file);
				}
				else
					fileSystemVisitor = new FileSystemVisitor();

				fileSystemVisitor.OnStart += FileSystemVisitor_OnStart;
				fileSystemVisitor.OnFinish += FileSystemVisitor_OnFinish;
				fileSystemVisitor.OnFileFound += FileSystemVisitor_OnFileFound;
				fileSystemVisitor.OnDirectoryFound += FileSystemVisitor_OnDirectoryFound;
				fileSystemVisitor.OnFilteredFileFound += FileSystemVisitor_OnFilteredFileFound;
				fileSystemVisitor.OnFilteredDirectoryFound += FileSystemVisitor_OnFilteredDirectoryFound;
				OnCancel += MainWindow_OnCancel;
				
				foreach (var item in fileSystemVisitor.SearchForFilesAndFolders(openFileDlg.SelectedPath))
				{
					ResultBox.Text += item;
					await Task.Delay(500); // Just to show events happening
					if (IsCancel) break;
				}
			}
		}

		private void FileSystemVisitor_OnFilteredDirectoryFound(object sender, EventArgs e)
		{
			FoundLabel.Content = "Filtered Directory Found";
		}

		private void FileSystemVisitor_OnFilteredFileFound(object sender, EventArgs e)
		{
			FoundLabel.Content = "Filtered File Found";
		}

		private void FileSystemVisitor_OnDirectoryFound(object sender, EventArgs e)
		{
			FoundLabel.Content = "Directory Found";
		}

		private void FileSystemVisitor_OnFileFound(object sender, EventArgs e)
		{
			FoundLabel.Content = "File Found";
		}

		private void FileSystemVisitor_OnStart(object sender, EventArgs e)
		{
			SearchLabel.Content = "Searching...";
			FilterBtn.IsEnabled = false;
			CancelBtn.IsEnabled = true;
			ResultBox.Text = "";
			IsCancel = false;
		}
		private void FileSystemVisitor_OnFinish(object sender, EventArgs e)
		{
			SearchLabel.Content = "Done.";
			FilterBtn.IsEnabled = true;
			CancelBtn.IsEnabled = false;
		}
		private void MainWindow_OnCancel(object sender, bool e)
		{
			IsCancel = e;
			SearchLabel.Content = "Canceled.";
			CancelBtn.IsEnabled = false;
			FilterBtn.IsEnabled = true;
		}

		private void CancelBtn_Click(object sender, RoutedEventArgs e)
		{
			OnCancel?.Invoke(this, true);
		}

		private void FilterBtn_Click(object sender, RoutedEventArgs e)
		{
			string filterString = FilterBox.Text;
			if (!string.IsNullOrWhiteSpace(filterString))
			{
				List<string> resultList = new List<string>(ResultBox.Text.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries));
				ResultBox.Text = "";
				foreach (var result in resultList)
				{

					if (!result.Contains(filterString))
					{
						ResultBox.Text += result + "\n";
					}

				}
			}
		}
	}
}
