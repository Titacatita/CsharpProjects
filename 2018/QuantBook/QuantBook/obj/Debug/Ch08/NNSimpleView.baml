   M S B A M L   `   `   `  ����D  @QuantBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuantBook.Models.ChartModelQuantBook.Models.ChartModel  5      !    QuantBook.Ch08.NNSimpleView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:QuantBook.Models.ChartModel     5      .��5	      � $��20q�6     � ColumnDefinitions  5
   
   �� 5      $	��250 �6   �� 5      .+�5   
   �� $��0 0 10 0q�6   .+�5      �� $��
Input Data��6   .��5      �� $��0 0 0 5q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Window Size:��6$   $	��120��68   $��Center=�6D    �� TextAlignment$ Right=�6_   �� 5      -   5         ��Name$ 
WindowSize��5   "   $��90��66    �� TextAlignment$ Center=�6A   $��10 0 0 0q�6X   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Prediction Size:��6$   $	��120��6<   $��Center=�6H   $ Right=�6c   �� 5      -   5        $ PredictionSize��5   "   $��90��6:   $ Center=�6E   $��10 0 0 0q�6\   5      	� 5      -   5         	�Name$ OriginalInput��5   "   $��Original Input��69   $��	20 10 0 0q�6R   	� 5      -   5        $ NormalizedInput��5   "   $��Normalized Input��6;   $��	20 10 0 0q�6V   �� 5      -   5         ��Name$ LoadData��5      $��	Load Data��6/   $	��150��6C   $��0 20 0 0q�6O   5      5      �� 5       $��Neural Network Settings��6   $��0 30 0 0q�68   .��5!      �� $��0 0 0 5q�6   .+�5"      �� $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5#      �� $��Learning Rate:��6$   $	��120��6:   $��Center=�6F   $ Right=�6a   �� 5$      -   5        $ LearningRate��5$   "   $��90��68   $ Center=�6C   $��10 0 0 0q�6Z   5%      �� 5&      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5'      �� $��Iterations:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 5(      -   5        $ 
Iterations��5(   "   $��90��66   $ Center=�6A   $��10 0 0 0q�6X   5)      �� 5*      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5+      �� $��Sigmoid's Alpha Value:��6$   $	��120��6B   $��Center=�6N   $ Right=�6i   �� 5,      -   5        $ Alpha��5,   "   $��90��61   $ Center=�6<   $��10 0 0 0q�6S   5-      �� 5.      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5/      �� $��Relative Error:��6$   $	��120��6;   $��Center=�6G   $ Right=�6b   �� 50      -	   5        $ RelativeError��50   "   $��90��69   $ Center=�6D   $��10 0 0 0q�6[   51      	� 52      -
   5        $ LMAlgorithm��52   "   $��LM Algorithm��67   $��0 10 0 0q�6N   	� 53      -   5        $ RpropAlgorithm��53   "   $��Resilient Backpropagation��6:   $��0 10 0 0q�6^   �� 54      -   5        $ StartNN��54      $��Start Neural Network��6.   $	��150��6M   $��0 20 0 0q�6Y   �� 55      -   5        $
 Stop��55      $
��Stop��6+   $	��150��6:   $��0 10 0 0q�6F   56      57      58      � 59   
   $��1��6     5:      �� 5;      �� 5<       � RowDefinitions 5>      �� 5?      �� 5@      .+�5C      � $��2��6     5D      �� 5E      $��1* �6'   �� 5F      $��4* �6'   .+�5H      &   System.Windows.Controls.DataGrid     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$ * 6   	   CanUserAddRows	 .  $��0��6a   
   SelectionUnit$

 Cell=�6q   ��6B   �� *Table1+6q     5I      $ * 6   	 .  $��1��6a   $

 Cell=�6q   ��6B   �� *Table2+6q   5J      )   #QuantBook.Models.ChartModel.MsChart  5L         Title$ Error Function��6S   $��0��6j   $��1��6z      XLabel$ 
Iterations��5M         YLabel$ Error��60      SeriesCollection 5L      �� *LineSeriesCollectionError+5M   0     5N         
XValueType$
 Date��6m   $ Regression Results��5O      $��1��67   $��1��6G   $ Index��6T   $ Results��6c    5N      �� *LineSeriesCollection1+   
DataSource 6O   �� *Table2+5O   c   5P      5Q      5R      