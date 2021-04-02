using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Rankinator
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		int TopBound { get; set; } = 0;
		int BotBound { get; set; } = 0;
		int CurrentComparison { get; set; } = -1;
		List<string> TheList { get; set; } = new List<string>();
		int ChoicesMade { get; set; } = 0;
		string Newthing { get; set; } = "NEW THING";
		int ListViewerTarget { get; set; } = 0;
		const string LISTFILENAME = "thelist.txt";
		const string INPUTFILENAME = "input.txt";
		bool AddingFromFile { get; set; } = false;

		public MainWindow()
		{
			InitializeComponent();
			LoadList();
			if (TheList.Count > 0)
			{
				RefreshListView();
			}
		}
		/// <summary>
		/// Begin the ranking of a new item.
		/// Immediately adds item to list if list is empty
		/// Switches to the ranking view.
		/// </summary>
		/// <param name="newItemName">Name of thing to be ranked.</param>
		void StartRankingNewItem(string newItemName)
		{
			SwitchToRankingView();
			ChoicesMade = 0;
			CurrentComparison = -1;
			TopBound = 0;
			BotBound = TheList.Count - 1;
			Newthing = newItemName;
			BarMessage.Content = $"Ranking {newItemName}";
			NextComparison();
		}
		/// <summary>
		/// Load the list from <c>LISTFILENAME</c>.
		/// </summary>
		void LoadList()
		{
			try
			{
				using (StreamReader sr = new StreamReader(LISTFILENAME))
				{
					while (!sr.EndOfStream)
					{
						string item = sr.ReadLine();
						if (!string.IsNullOrWhiteSpace(item))
						{
							TheList.Add(item);
						}
					}
				}
				BarMessage.Content = "List loaded";
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		/// <summary>
		/// Load list of items to be ranked from <c>INPUTFILENAME</c>.
		/// </summary>
		/// <returns>List of strings</returns>
		List<string> LoadInput()
		{
			List<string> inputs = new List<string>();
			try
			{
				using (StreamReader sr = new StreamReader(INPUTFILENAME))
				{
					while (!sr.EndOfStream)
					{
						string item = sr.ReadLine();
						if (!string.IsNullOrWhiteSpace(item))
						{
							inputs.Add(item);
						}
					}
				}
				if (inputs.Count == 0)
				{
					MessageBox.Show($"{INPUTFILENAME} appears to be empty.");
				}
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
			return inputs;
		}
		/// <summary>
		/// Load <c>INPUTFILENAME</c>, excluding the given item, and save it back
		/// </summary>
		/// <param name="item">Item to remove</param>
		void RemoveItemFromInput(string removeTarget)
		{
			List<string> inputs = new List<string>();
			try
			{
				using (StreamReader sr = new StreamReader(INPUTFILENAME))
				{
					while (!sr.EndOfStream)
					{
						string readItem = sr.ReadLine();
						if (!string.IsNullOrWhiteSpace(readItem) && readItem != removeTarget)
						{
							inputs.Add(readItem);
						}
					}
				}
				using (var sw = new StreamWriter(INPUTFILENAME))
				{
					foreach (var item in inputs)
					{
						sw.WriteLine(item);
					}
				}
				BarMessage.Content += $" | {removeTarget} removed from {INPUTFILENAME}";
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		/// <summary>
		/// Save the list to <c>LISTFILENAME</c>
		/// </summary>
		void SaveList()
		{
			try
			{
				using (var sw = new StreamWriter(LISTFILENAME))
				{
					foreach (var item in TheList)
					{
						sw.WriteLine(item);
					}
				}
				BarMessage.Content += " | List saved.";
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message);
			}
		}
		/// <summary>
		/// Things to do with the list view
		/// </summary>
		/// <param name="selectedIndex">Select and scroll to this index</param>
		void RefreshListView(int selectedIndex = 0)
		{
			ListViewerTarget = selectedIndex;
			ListViewer.ItemsSource = null;
			ListViewer.ItemsSource = TheList;
			ListViewer.SelectedIndex = selectedIndex;
			if (selectedIndex >= 0 && selectedIndex < ListViewer.Items.Count)
			{
				ListViewer.ScrollIntoView(ListViewer.Items[selectedIndex]); 
			}
		}
		/// <summary>
		/// Update numbers and progress bar in the status bar
		/// </summary>
		void UpdateStatusBar()
		{
			BarBotBound.Content = BotBound;
			BarTopBound.Content = TopBound;
			BarCurComp.Content = CurrentComparison;
			BarListLength.Content = TheList.Count;

			StatusProgress.Maximum = Math.Ceiling(Math.Log2(TheList.Count));
			StatusProgress.Value = ChoicesMade;
		}

		private void BetterButton_Click(object sender, RoutedEventArgs e)
		{
			BotBound = CurrentComparison - 1;
			ChoicesMade++;
			NextComparison();
		}

		private void WorseButton_Click(object sender, RoutedEventArgs e)
		{
			TopBound = CurrentComparison + 1;
			ChoicesMade++;
			NextComparison();
		}
		/// <summary>
		/// Set CurrentComparison and button labels.
		/// Insert into the list if it's time.
		/// </summary>
		void NextComparison()
		{
			CurrentComparison = (int)Math.Floor((BotBound - TopBound) * 0.5f + TopBound);
			if (TopBound > BotBound)
			{
				TheList.Insert(TopBound, Newthing);
				BarMessage.Content = $"{Newthing} was added at position {TopBound}";
				if (AddingFromFile && RemoveCheckbox.IsChecked == true)
				{
					RemoveItemFromInput(Newthing);
				}
				SaveList();
				SwitchToNewItemView();
				RefreshListView(TopBound);
				UpdateStatusBar();
				return;
			}
			BetterButtonText.Text = Newthing;
			WorseButtonText.Text = TheList[CurrentComparison];
			UpdateStatusBar();
			RefreshListView(CurrentComparison);
		}
		/// <summary>
		/// Switch to the new item view
		/// </summary>
		private void SwitchToNewItemView()
		{
			NewItemGrid.Visibility = Visibility.Visible;
			NewItemGrid.IsEnabled = true;
			BetterWorseGrid.Visibility = Visibility.Hidden;
			BetterWorseGrid.IsEnabled = false;
		}
		/// <summary>
		/// Switch to the ranking view
		/// </summary>
		private void SwitchToRankingView()
		{
			NewItemGrid.Visibility = Visibility.Hidden;
			NewItemGrid.IsEnabled = false;
			BetterWorseGrid.Visibility = Visibility.Visible;
			BetterWorseGrid.IsEnabled = true;
		}
		/// <summary>
		/// I really want the list to keep the same thing selected.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ListViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListViewer.SelectedIndex = ListViewerTarget;
		}

		private void AddTextboxButton_Click(object sender, RoutedEventArgs e)
		{
			string newItem = CustomItemText.Text;
			if (!string.IsNullOrWhiteSpace(newItem))
			{
				AddingFromFile = false;
				StartRankingNewItem(newItem);
				CustomItemText.Text = "TextBox";
			}
			else
			{
				MessageBox.Show("Please type something in the box.");
			}
		}

		private void AddTopButton_Click(object sender, RoutedEventArgs e)
		{
			var input = LoadInput();
			if (input.Count > 0)
			{
				AddingFromFile = true;
				StartRankingNewItem(LoadInput()[0]);
			}
		}

		private void AddRandomButton_Click(object sender, RoutedEventArgs e)
		{
			var input = LoadInput();
			if (input.Count > 0)
			{
				Random rng = new Random();
				AddingFromFile = true;
				StartRankingNewItem(LoadInput()[rng.Next(0, input.Count - 1)]);
			}
		}

		private void OpenList_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				var p = new Process();
				p.StartInfo.FileName = LISTFILENAME;
				p.StartInfo.UseShellExecute = true;
				p.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		private void OpenInput_Click(object sender, RoutedEventArgs e)
		{
			if (!File.Exists(INPUTFILENAME))
			{
				var foo = File.CreateText(INPUTFILENAME);
				foo.Close();
			}
			try
			{
				var p = new Process();
				p.StartInfo.FileName = INPUTFILENAME;
				p.StartInfo.UseShellExecute = true;
				p.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
