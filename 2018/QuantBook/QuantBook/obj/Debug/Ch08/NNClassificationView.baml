   M S B A M L   `   `   `  ����D  @QuantBook, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nullI)clr-namespace:QuantBook.Models.ChartModelQuantBook.Models.ChartModel  5      )    #QuantBook.Ch08.NNClassificationView   5      R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      ;local)clr-namespace:QuantBook.Models.ChartModel     5      .��5	      � $��20q�6     � ColumnDefinitions  5
   
   �� 5      $	��250 �6   �� 5      .+�5   
   �� $��0 0 10 0q�6   .+�5      �� $��
Input Data��6   .��5      �� $��0 0 0 5q�6   .+�5      ��  �� Orientation$ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Ticker:��6$   $	��120��63   $��Center=�6?    �� TextAlignment$ Right=�6Z   �� 5      -   5         ��Name$ Ticker��5   "   $��90��62    �� TextAlignment$ Center=�6=   $��10 0 0 0q�6T   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Start Date:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 5      -   5        $ 	StartDate��5   "   $��90��65   $ Center=�6@   $��10 0 0 0q�6W   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��	End Date:��6$   $	��120��65   $��Center=�6A   $ Right=�6\   �� 5      -   5        $ EndDate��5   "   $��90��63   $ Center=�6>   $��10 0 0 0q�6U   5      �� 5      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5      �� $��Training Start Date:��6$   $	��120��6@   $��Center=�6L   $ Right=�6g   �� 5      -   5        $ TrainStartDate��5   "   $��90��6:   $ Center=�6E   $��10 0 0 0q�6\   5       �� 5!      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5"      �� $��Training End Date:��6$   $	��120��6>   $��Center=�6J   $ Right=�6e   �� 5#      -   5        $ TrainEndDate��5#   "   $��90��68   $ Center=�6C   $��10 0 0 0q�6Z   5$      �� 5%      -   5         ��Name$ LoadData��5%      $��	Load Data��6/   $	��150��6C   $��0 20 0 0q�6O   5&      5'      �� 5)      $��Neural Network Settings��6   $��0 30 0 0q�68   .��5*      �� $��0 0 0 5q�6   .+�5+      �� $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5,      �� $��Learning Rate:��6$   $	��120��6:   $��Center=�6F   $ Right=�6a   �� 5-      -   5        $ LearningRate��5-   "   $��90��68   $ Center=�6C   $��10 0 0 0q�6Z   5.      �� 5/      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�50      �� $��Iterations:��6$   $	��120��67   $��Center=�6C   $ Right=�6^   �� 51      -   5        $ 
Iterations��51   "   $��90��66   $ Center=�6A   $��10 0 0 0q�6X   52      �� 53      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�54      �� $��Neurons in Hidden Layer:��6$   $	��120��6D   $��Center=�6P   $ Right=�6k   �� 55      -	   5        $ HiddenNeurons��55   "   $��90��69   $ Center=�6D   $��10 0 0 0q�6[   56      �� 57      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�58      �� $��Sigmoid's Alpha Value:��6$   $	��120��6B   $��Center=�6N   $ Right=�6i   �� 59      -
   5        $ Alpha��59   "   $��90��61   $ Center=�6<   $��10 0 0 0q�6S   5:      �� 5;      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5<      �� $��Number of Classes:��6$   $	��120��6>   $��Center=�6J   $ Right=�6e   �� 5=      -   5        $ 
NumClasses��5=   "   $��90��66   $ Center=�6A   $��10 0 0 0q�6X   5>      �� 5?      $ 
Horizontal=�6!   $��0 5 0 0q�6:   .+�5@      �� $��Relative Error:��6$   $	��120��6;   $��Center=�6G   $ Right=�6b   �� 5A      -   5        $ RelativeError��5A   "   $��90��69   $ Center=�6D   $��10 0 0 0q�6[   5B      	� 5C      -   5         	�Name$ LMAlgorithm��5C   "   $��LM Algorithm��67   $��0 10 0 0q�6N   	� 5D      -   5        $ RpropAlgorithm��5D   "   $��Resilient Backpropagation��6:   $��0 10 0 0q�6^   �� 5E      -   5        $ StartNN��5E      $��Start Neural Network��6.   $	��150��6M   $��0 20 0 0q�6Y   �� 5F      -   5        $
 Stop��5F      $
��Stop��6+   $	��150��6:   $��0 10 0 0q�6F   5G      5H      5I      � 5J   
   $��1��6     5K      �� 5L      �� 5M       � RowDefinitions 5O      �� 5P      �� 5Q      .+�5T      &   System.Windows.Controls.DataGrid     ColumnWidth5  /System.Windows.Controls.DataGridLengthConverter$
 Auto 6   	   CanUserAddRows	 .  $��0��6`   
   SelectionUnit$

 Cell=�6p   ��6A   �� *Table1+6p     5U      $
 Auto 6   	 .  $��1��6`   $

 Cell=�6p   ��6A   �� *Table2+6p   )   #QuantBook.Models.ChartModel.MsChart  5W         Title$ Error Function��6S   $��0��6j   $��1��6z      XLabel$ 
Iterations��5X         YLabel$ Error��60      SeriesCollection 5W      �� *LineSeriesCollectionError+5X   0   � 5Y      $��1��6   $��1��6#    5Z      �� 5[      �� 5\      .+�5^      �� $��0��6   .+�5_      �� $-��'Confusion Matrix for Training Data Set:��6    $��0 0 0 2q�6O     5`      $
 Auto 6   	 .  $

 Cell=�6h   ��6I   �� *Table3+6h   5a      �� 5b      $��1��6   .+�5c      �� $/��)Confusion Matrix for Prediction Data Set:��6    $��0 0 0 2q�6Q     5d      $
 Auto 6   	 .  $

 Cell=�6h   ��6I   �� *Table4+6h   5e      5f      5g      5h      5i      