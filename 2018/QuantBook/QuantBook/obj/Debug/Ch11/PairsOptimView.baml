   M S B A M L   `   `   `  ����D  @QuantBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuantBook.Models.ChartModelQuantBook.Models.ChartModel  5      #    QuantBook.Ch11.PairsOptimView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ^ ZCaliburn.Micro.Platform, Version=2.0.2.0, Culture=neutral, PublicKeyToken=8e5891231f2ed21f(calhttp://www.caliburnproject.org  5      5local)clr-namespace:QuantBook.Models.ChartModel  5      .��5
      � $��20q�6     � ColumnDefinitions  5   
   �� 5      $	��250 �6   �� 5      .+�5   
   �� $��0 0 10 0q�6   .+�5      �� $��Get Market Data��6   .��5      �� $��0 0 0 5q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Ticker1:��6$   $	��120��64   $��Center=�6@    �� TextAlignment$ Right=�6[   �� 5      -   5         ��Name$ Ticker1��5   "   $��90��63    �� TextAlignment$ Center=�6>   $��10 0 0 0q�6U   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Ticker2:��6$   $	��120��64   $��Center=�6@   $ Right=�6[   �� 5      -   5        $ Ticker2��5   "   $��90��63   $ Center=�6>   $��10 0 0 0q�6U   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Start Date:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 5      -   5        $ 	StartDate��5   "   $��90��65   $ Center=�6@   $��10 0 0 0q�6W   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��	End Date:��6$   $	��120��65   $��Center=�6A   $ Right=�6\   �� 5       -   5        $ EndDate��5    "   $��90��63   $ Center=�6>   $��10 0 0 0q�6U   5!      �� 5"      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5#      �� $��Correlation Window:��6$   $	��120��6?   $��Center=�6K   $ Right=�6f   �� 5$      -   5        $ CorrelWindow��5$   "   $��90��68   $ Center=�6C   $��10 0 0 0q�6Z   5%      �� 5'      -   5         ��Name$ GetData��5'      $��Get Data��6.   $	��150��6A   $��0 20 0 0q�6M   5(      5)      �� 5+      $��Optimization��6   $��0 30 0 0q�6-   .��5,      �� $��0 5 0 5q�6   .+�5-      �� $ 
Horizontal=�6!   $��0 0 0 0q�6:   .+�5.      �� $��
Pair Type:��6$   $	��120��66   $��Center=�6B   $ Right=�6]   �� 5/      -   5         ��Name$ PairType��5/   #   $��90��65   $��10 0 0 0q�6A   50      �� 51      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�52      �� $��Hedge Ratio:��6$   $	��120��68   $��Center=�6D   $ Right=�6_   �� 53      -   5        $ 
HedgeRatio��53   "   $��90��66   $ Center=�6A   $��10 0 0 0q�6X   54      �� 56      -	   5        $ 
StartOptim��56      $��Start Optimization��61   $	��150��6N   $��0 20 0 0q�6Z   57      58      5:      � 5;   
   $��1��6    � RowDefinitions 5<      �� 5=      $��1* �6    �� 5>      $��2* �6    .+�5@      �   5A      �� 5B      $��1* �6'   �� 5C      $��2* �6'   .+�5E      &   System.Windows.Controls.DataGrid  -
   5          Name$ 
OptimTable��5E      	   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$	 * 6/   
   CanUserAddRows
 .     SelectionUnit$ FullRow=�6V     Caliburn.Micro.Message   Attach$V P[Event SelectedCellsChanged] = [Action SelectedCellChanged($source, $eventArgs)]��6n     5F      -   5        $ PnLCollection��5F      $	 * 62   
 .  $
 Cell=�6Y   $��1��6n   5G      � 5H      $��1��6    5I      �� 5J      �� 5K        5M      �� 5N      �� 5O      �� 5P      .+�5R      )   #QuantBook.Models.ChartModel.MsChart  $��0��6l   $��0��6|      XLabel$
 Date��6�      
XValueType$
 Date��6�      YLabel$ Stock Price��6�      Y2Label$ Stock Price��6�      SeriesCollection 6    �� *LineSeriesCollection1+   Title 6S   �� *Title1+6�     5S      $��0��6l   $��1��6|   $
 Date��6�   $
 Date��6�   $ Correlation��6�    6    �� *LineSeriesCollection2+ 6S   �� *Title2+6�     5T      $��1��6l   $��0��6|   $
 Date��6�   $
 Date��6�   $
 Beta��6�    6    �� *LineSeriesCollection3+ 6S   �� *Title3+6�     5U      $��1��6l   $��1��6|   $
 Date��6�   $
 Date��6�   $ Signal��6�    6    �� *LineSeriesCollection4+ 6S   �� *Title4+6�     5V      $��2��6l   $��0��6|   $
 Date��6�   $
 Date��6�   $	 PnL��6�    6    �� *LineSeriesCollection5+ 6S   �� *Title5+6�     5W      $��2��6l   $��1��6|   $
 Date��6�   $
 Date��6�   $ Drawdown (%)��6�    6    �� *LineSeriesCollection6+ 6S   �� *Title6+6�   5X      5Y      5Z      5[      