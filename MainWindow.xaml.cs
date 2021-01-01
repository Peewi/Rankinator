using System;
using System.Collections.Generic;
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
		int LastComparison { get; set; } = -1;
		int CurrentComparison { get; set; } = -1;
		int WorseAdjust { get; set; } = 0;
		List<string> TheList { get; set; } = new List<string>();
		List<int> ComparedItems { get; set; } = new List<int>();
		bool RankingStarted { get; set; } = false;
		string Newthing { get; set; } = "NEW THING";
		public string MyProperty { get; set; } = "initial";

		public MainWindow()
		{
			InitializeComponent();
			TheList.Add("ONE");
			//TheList.Add("TWO");
			NewItem();
		}

		void NewItem()
		{
			ComparedItems.Clear();
			RankingStarted = false;
			LastComparison = -1;
			CurrentComparison = -1;
			TopBound = 0;
			BotBound = TheList.Count - 1;
			Newthing = $"Thing {TheList.Count}";
			NextComparison();
		}

		void UpdateStatusBar()
		{
			BarBotBound.Content = BotBound;
			BarTopBound.Content = TopBound;
			BarCurComp.Content = CurrentComparison;
			BarLastComp.Content = LastComparison;
			BarListLength.Content = TheList.Count;
			ListViewer.ItemsSource = null;
			ListViewer.ItemsSource = TheList;
			ListViewer.SelectedIndex = CurrentComparison;
			ListViewer.ScrollIntoView(TheList[CurrentComparison]);
		}

		private void BetterButton_Click(object sender, RoutedEventArgs e)
		{
			RankingStarted = true;
			BotBound = CurrentComparison;
			WorseAdjust = 0;
			NextComparison();
		}

		private void WorseButton_Click(object sender, RoutedEventArgs e)
		{
			RankingStarted = true;
			TopBound = CurrentComparison;
			WorseAdjust = 1;
			NextComparison();
		}

		void NextComparison()
		{
			LastComparison = CurrentComparison;
			ComparedItems.Add(LastComparison);
			CurrentComparison = (int)Math.Ceiling((BotBound - TopBound) * 0.5f + TopBound);
			if (CurrentComparison == LastComparison)
			{
				CurrentComparison--;
			}
			if ((TopBound == BotBound && RankingStarted)
				|| (BotBound - TopBound == 1 && ComparedItems.Contains(TopBound) && ComparedItems.Contains(BotBound))
				)
			{
				//MessageBox.Show($"{newthing} was added at position {lastComparison + worseAdjust}");
				TheList.Insert(LastComparison + WorseAdjust, Newthing);
				NewItem();
				return;
			}
			BetterButton.Content = Newthing;
			WorseButton.Content = TheList[CurrentComparison];
			UpdateStatusBar();
		}

		private void ListViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			ListViewer.SelectedIndex = CurrentComparison;
		}
	}
}
