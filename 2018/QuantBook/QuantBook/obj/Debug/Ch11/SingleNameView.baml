   M S B A M L   `   `   `  ����D  @QuantBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuantBook.Models.ChartModelQuantBook.Models.ChartModel  5      #    QuantBook.Ch11.SingleNameView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:QuantBook.Models.ChartModel     5      .��5	      � $��20q�6     � ColumnDefinitions  5
   
   �� 5      $	��250 �6   �� 5      .+�5   
   �� $��0 0 10 0q�6   .+�5      �� $��Get Market Data��6   .��5      �� $��0 0 0 5q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Ticker:��6$   $	��120��63   $��Center=�6?    �� TextAlignment$ Right=�6Z   �� 5      -   5         ��Name$ Ticker��5   "   $��90��62    �� TextAlignment$ Center=�6=   $��10 0 0 0q�6T   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Start Date:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 5      -   5        $ 	StartDate��5   "   $��90��65   $ Center=�6@   $��10 0 0 0q�6W   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��	End Date:��6$   $	��120��65   $��Center=�6A   $ Right=�6\   �� 5      -   5        $ EndDate��5   "   $��90��63   $ Center=�6>   $��10 0 0 0q�6U   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Moving Window:��6$   $	��120��6:   $��Center=�6F   $ Right=�6a   �� 5      -   5        $ MovingWindow��5   "   $��90��68   $ Center=�6C   $��10 0 0 0q�6Z   5       �� 5!      $��Select Price Type:��6    $��0 10 0 0q�6:   �� 5"      -   5         ��Name$ 	PriceType��5"      $	��200��62   $��0 2 0 0q�6>   �� 5#      $��Select Signal Type:��6    $��0 10 0 0q�6;   �� 5$      -   5        $ 
SignalType��5$      $	��200��63   $��0 2 0 0q�6?   �� 5%      -   5         ��Name$ GetSignalData��5%      $��Get Signal Data��64   $	��150��6N   $��0 20 0 0q�6Z   5'      5(      �� 5*      $��Compute PnL��6   $��0 20 0 0q�6,   .��5+      �� $��0 5 0 5q�6   .+�5,      �� $ 
Horizontal=�6!   $��0 0 0 0q�6:   .+�5-      �� $��
Signal In:��6$   $	��120��66   $��Center=�6B   $ Right=�6]   �� 5.      -   5        $ SignalIn��5.   "   $��90��64   $ Center=�6?   $��10 0 0 0q�6V   5/      �� 50      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�51      �� $��Signal Out:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 52      -	   5        $ 	SignalOut��52   "   $��90��65   $ Center=�6@   $��10 0 0 0q�6W   53      �� 54      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�55      �� $��	Notional:��6$   $	��120��65   $��Center=�6A   $ Right=�6\   �� 56      -
   5        $ Notional��56   "   $��90��64   $ Center=�6?   $��10 0 0 0q�6V   57      �� 58      -   5         ��Name$ 
IsReinvest��58      $��Is Re-Invest?��63   $��	20 10 0 0q�6K   �� 59      $��Select Strategy Category:��6    $��0 10 0 0q�6A   �� 5:      -   5        $ StrategyType��5:      $	��200��65   $��0 2 0 0q�6A   �� 5;      -   5        $ 
ComputePnL��5;      $��Compute PnL��61   $	��150��6G   $��0 20 0 0q�6S   �� 5<      -   5        $ DrawdownStrategy��5<      $��Drawdown for Strategy��67   $	��150��6W   $��0 10 0 0q�6c   �� 5=      -   5        $ DrawdownHold��5=      $��Drawdown for Holding��63   $	��150��6R   $��0 10 0 0q�6^   5>      5?      5@      � 5A   
   $��1��6     5B      �� 5C      �� 5D       � RowDefinitions 5F      �� 5G      �� 5H      .+�5K      &   System.Windows.Controls.DataGrid  -   5        	  Name$	 SignalCollection��5K      
   ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$
 * 61      CanUserAddRows .  $��0��6X      SelectionUnit$
 Cell=�6h   � 5L      $��1��6    5M      �� 5N      $��3* �6$   �� 5O      $��2* �6$   .+�5Q        -   5        $	 PnLCollection��5Q      $
 * 62    .  $
 Cell=�6Y     5R      -   5        $	 YearlyPnLTable��5R      $
 * 63    .  $��1��6Z   $
 Cell=�6g   5S      )   #QuantBook.Models.ChartModel.MsChart  5T      $��0��6h   $��1��6x      XLabel$
 Date��6�      
XValueType$
 Date��6�      SeriesCollection 6   �� *LineSeriesCollection1+   Title 6O   �� *Title1+   YLabel 6�   �� *	YLabel1+  5U      $��1��6h   $��1��6x   $
 Date��6�   $
 Date��6�    6   �� *LineSeriesCollection2+ 6O   �� *Title2+ 6�   �� *	YLabel2+5W      5X      5Y      