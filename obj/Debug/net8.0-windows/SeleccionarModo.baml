   M S B A M L   `   `   `  ����  Snake2D    Snake2D.SeleccionarModo   5   	   W SPresentationCore, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=8.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      5   	   R NSystem.Xaml, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  Seleccionar Modo de Juego��5   	   $	��600��6+   $	��600��68    � WindowStartupLocation$ CenterScreen=�5   	    � 
ResizeMode$ NoResize=�6.    � WindowStyle$
 None=�5   	    � 	Resources 5         BotonConEstilo �� 
TargetType �� Property �� Value �� 
TargetType	 ��Name
 �� CornerRadius J� Effect3  -System.Windows.Media.Effects.DropShadowEffect   Color   
BlurRadius   ShadowDepth �� Triggers P� Property P� Value �� 
TargetName��%�  &	        �� 5	   
   " ��6'   .�5
      ��  � ��
 �����6+   �� 5       � ��$
 Bold"�6+   �� 5       � ��$ 18(�6)   �� 5       � ��$ 10q�6'   �� 5       � ��$
 10,5q�6(   �� 5       � �� �	#FF2196F36+   �� 5       � ��
 ���� 6,   �� 5       �@�Cursor$
 Hand��6'   �� 5       � �� 5      �� 5      " ��6&   .R�5      � .+�5      �� $	 border��6%   #��z��65   $
 10��5   %   $��0q�67    5   "     5   &   $ Black��67   $ 15Q�6E   $ 3Q�6U   5      �� 5      $��Center=�6/   $��Center=�6L   5       5      P� 5       �@J�IsMouseOver$
 True��6=   . �5    "   �� $ border��6)    � �� �	#FF1976D26S   5!      P� 5"       � ��$
 True��6;   . �5#   "   �� $ border��6)    � �� �	#FF0D47A16S   5$      5&      5(      5)      5*      .��5+      � -   5        ��5-   
   �� 5.       �� 
StartPoint$	 0,0&�6"    �� EndPoint$	 0,1&�63   .��5/      �  � Color$ 	#FF2196F3��6    � Offset$	 0.0Q�61   � 50      $ 	#FF0D47A1��6   $	 1.0Q�61   51      .+�54   
   �� ���	#AAFFFFFF$
 10��6(    �� Padding$ 20q�6:   $��50q�6G   ._�55      � .+�57      �� -   5        $��X��57      $��30��6%   $��30��60   $��Right=�6<   $	��Top=�6X   
������ 
������ 
���   �$
��Bold"�59   ,   $��16(�6>    � Cursor$
 Hand��5:      �� 5<      $��Center=�6   .+�5=      �� $"��Seleccione el modo de juego:��6    $
��Bold"�5>      $��24(�6"    �� TextAlignment$ Center=�60   $��	0,30,0,20q�6G   �� 5?      -   5        $��BotonUnJugador��5?      $��	1 Jugador��63   $��10q�6d   #��[  5@      $	��250��65   $��60��6A   �� 5A      -   5        $��BotonDosJugadores��5A      $��2 Jugadores��66   $��10q�6l   #��[  5B      $	��250��65   $��60��6A   �� 5C      -   5        $��BotonContraIA��5C      $��Jugar vs IA��62   $��10q�6d   #��[  5D      $	��250��65   $��60��6A   �� 5E      -   5        $��
BotonSalir��5E      $��Salir del Juego��6/   $��10q�6b   #��[  5F      $	��250��65   $��60��6A   5G      5I      5J      5K      5L      