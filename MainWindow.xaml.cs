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
		int lastComparison = -1;
		int currentComparison = -1;
		int worseAdjust = 0;
		List<string> TheList = new List<string>();
		List<int> ComparedItems = new List<int>();
		bool RankingStarted = false;
		string newthing = "NEW THING";
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
			lastComparison = -1;
			currentComparison = -1;
			TopBound = 0;
			BotBound = TheList.Count - 1;
			newthing = $"Thing {TheList.Count}";
			NextComparison();
		}

		void UpdateStatusBar()
		{
			BarBotBound.Content = BotBound;
			BarTopBound.Content = TopBound;
			BarCurComp.Content = currentComparison;
			BarLastComp.Content = lastComparison;
			BarListLength.Content = TheList.Count;
			ListViewer.ItemsSource = null;
			ListViewer.ItemsSource = TheList;
			ListViewer.SelectedIndex = currentComparison;
		}

		private void BetterButton_Click(object sender, RoutedEventArgs e)
		{
			RankingStarted = true;
			BotBound = currentComparison;
			worseAdjust = 0;
			NextComparison();
		}

		private void WorseButton_Click(object sender, RoutedEventArgs e)
		{
			RankingStarted = true;
			TopBound = currentComparison;
			worseAdjust = 1;
			NextComparison();
		}

		void NextComparison()
		{
			lastComparison = currentComparison;
			ComparedItems.Add(lastComparison);
			currentComparison = (int)Math.Ceiling((BotBound - TopBound) * 0.5f + TopBound);
			if (currentComparison == lastComparison)
			{
				currentComparison--;
			}
			UpdateStatusBar();
			if ((TopBound == BotBound && RankingStarted)
				|| (BotBound - TopBound == 1 && ComparedItems.Contains(TopBound) && ComparedItems.Contains(BotBound))
				)
			{
				MessageBox.Show($"{newthing} was added at position {lastComparison + worseAdjust}");
				TheList.Insert(lastComparison + worseAdjust, newthing);
				NewItem();
				return;
			}
			BetterButton.Content = newthing;
			WorseButton.Content = TheList[currentComparison];
		}
	}
}
