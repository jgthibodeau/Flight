/ /   U p g r a d e   N O T E :   r e p l a c e d   ' _ O b j e c t 2 W o r l d '   w i t h   ' u n i t y _ O b j e c t T o W o r l d ' 
 
 S h a d e r   " F X / W a t e r 4 "   { 
 P r o p e r t i e s   { 
 	 _ R e f l e c t i o n T e x   ( " I n t e r n a l   r e f l e c t i o n " ,   2 D )   =   " w h i t e "   { } 
 	 
 	 _ M a i n T e x   ( " F a l l b a c k   t e x t u r e " ,   2 D )   =   " b l a c k "   { } 
 	 _ S h o r e T e x   ( " S h o r e   &   F o a m   t e x t u r e   " ,   2 D )   =   " b l a c k "   { } 
 	 _ B u m p M a p   ( " N o r m a l s   " ,   2 D )   =   " b u m p "   { } 
 	 
 	 _ D i s t o r t P a r a m s   ( " D i s t o r t i o n s   ( B u m p   w a v e s ,   R e f l e c t i o n ,   F r e s n e l   p o w e r ,   F r e s n e l   b i a s ) " ,   V e c t o r )   =   ( 1 . 0   , 1 . 0 ,   2 . 0 ,   1 . 1 5 ) 
 	 _ I n v F a d e P a r e m e t e r   ( " A u t o   b l e n d   p a r a m e t e r   ( E d g e ,   S h o r e ,   D i s t a n c e   s c a l e ) " ,   V e c t o r )   =   ( 0 . 1 5   , 0 . 1 5 ,   0 . 5 ,   1 . 0 ) 
 	 
 	 _ A n i m a t i o n T i l i n g   ( " A n i m a t i o n   T i l i n g   ( D i s p l a c e m e n t ) " ,   V e c t o r )   =   ( 2 . 2   , 2 . 2 ,   - 1 . 1 ,   - 1 . 1 ) 
 	 _ A n i m a t i o n D i r e c t i o n   ( " A n i m a t i o n   D i r e c t i o n   ( d i s p l a c e m e n t ) " ,   V e c t o r )   =   ( 1 . 0   , 1 . 0 ,   1 . 0 ,   1 . 0 ) 
 
 	 _ B u m p T i l i n g   ( " B u m p   T i l i n g " ,   V e c t o r )   =   ( 1 . 0   , 1 . 0 ,   - 2 . 0 ,   3 . 0 ) 
 	 _ B u m p D i r e c t i o n   ( " B u m p   D i r e c t i o n   &   S p e e d " ,   V e c t o r )   =   ( 1 . 0   , 1 . 0 ,   - 1 . 0 ,   1 . 0 ) 
 	 
 	 _ F r e s n e l S c a l e   ( " F r e s n e l S c a l e " ,   R a n g e   ( 0 . 1 5 ,   4 . 0 ) )   =   0 . 7 5 
 
 	 _ B a s e C o l o r   ( " B a s e   c o l o r " ,   C O L O R )     =   (   . 5 4 ,   . 9 5 ,   . 9 9 ,   0 . 5 ) 
 	 _ R e f l e c t i o n C o l o r   ( " R e f l e c t i o n   c o l o r " ,   C O L O R )     =   (   . 5 4 ,   . 9 5 ,   . 9 9 ,   0 . 5 ) 
 	 _ S p e c u l a r C o l o r   ( " S p e c u l a r   c o l o r " ,   C O L O R )     =   (   . 7 2 ,   . 7 2 ,   . 7 2 ,   1 ) 
 	 
 	 _ W o r l d L i g h t D i r   ( " S p e c u l a r   l i g h t   d i r e c t i o n " ,   V e c t o r )   =   ( 0 . 0 ,   0 . 1 ,   - 0 . 5 ,   0 . 0 ) 
 	 _ S h i n i n e s s   ( " S h i n i n e s s " ,   R a n g e   ( 2 . 0 ,   5 0 0 . 0 ) )   =   2 0 0 . 0 
 	 
 	 _ F o a m   ( " F o a m   ( i n t e n s i t y ,   c u t o f f ) " ,   V e c t o r )   =   ( 0 . 1 ,   0 . 3 7 5 ,   0 . 0 ,   0 . 0 ) 
 	 
 	 _ G e r s t n e r I n t e n s i t y ( " P e r   v e r t e x   d i s p l a c e m e n t " ,   F l o a t )   =   1 . 0 
 	 _ G A m p l i t u d e   ( " W a v e   A m p l i t u d e " ,   V e c t o r )   =   ( 0 . 3   , 0 . 3 5 ,   0 . 2 5 ,   0 . 2 5 ) 
 	 _ G F r e q u e n c y   ( " W a v e   F r e q u e n c y " ,   V e c t o r )   =   ( 1 . 3 ,   1 . 3 5 ,   1 . 2 5 ,   1 . 2 5 ) 
 	 _ G S t e e p n e s s   ( " W a v e   S t e e p n e s s " ,   V e c t o r )   =   ( 1 . 0 ,   1 . 0 ,   1 . 0 ,   1 . 0 ) 
 	 _ G S p e e d   ( " W a v e   S p e e d " ,   V e c t o r )   =   ( 1 . 2 ,   1 . 3 7 5 ,   1 . 1 ,   1 . 5 ) 
 	 _ G D i r e c t i o n A B   ( " W a v e   D i r e c t i o n " ,   V e c t o r )   =   ( 0 . 3   , 0 . 8 5 ,   0 . 8 5 ,   0 . 2 5 ) 
 	 _ G D i r e c t i o n C D   ( " W a v e   D i r e c t i o n " ,   V e c t o r )   =   ( 0 . 1   , 0 . 9 ,   0 . 5 ,   0 . 5 ) 
 } 
 
 
 C G I N C L U D E 
 
 	 # i n c l u d e   " U n i t y C G . c g i n c " 
 	 # i n c l u d e   " W a t e r I n c l u d e . c g i n c " 
 
 	 s t r u c t   a p p d a t a 
 	 { 
 	 	 f l o a t 4   v e r t e x   :   P O S I T I O N ; 
 	 	 f l o a t 3   n o r m a l   :   N O R M A L ; 
 	 } ; 
 
 	 / /   i n t e r p o l a t o r   s t r u c t s 
 	 
 	 s t r u c t   v 2 f 
 	 { 
 	 	 f l o a t 4   p o s   :   S V _ P O S I T I O N ; 
 	 	 f l o a t 4   n o r m a l I n t e r p o l a t o r   :   T E X C O O R D 0 ; 
 	 	 f l o a t 4   v i e w I n t e r p o l a t o r   :   T E X C O O R D 1 ; 
 	 	 f l o a t 4   b u m p C o o r d s   :   T E X C O O R D 2 ; 
 	 	 f l o a t 4   s c r e e n P o s   :   T E X C O O R D 3 ; 
 	 	 f l o a t 4   g r a b P a s s P o s   :   T E X C O O R D 4 ; 
 	 	 U N I T Y _ F O G _ C O O R D S ( 5 ) 
 	 } ; 
 
 	 s t r u c t   v 2 f _ n o G r a b 
 	 { 
 	 	 f l o a t 4   p o s   :   S V _ P O S I T I O N ; 
 	 	 f l o a t 4   n o r m a l I n t e r p o l a t o r   :   T E X C O O R D 0 ; 
 	 	 f l o a t 3   v i e w I n t e r p o l a t o r   :   T E X C O O R D 1 ; 
 	 	 f l o a t 4   b u m p C o o r d s   :   T E X C O O R D 2 ; 
 	 	 f l o a t 4   s c r e e n P o s   :   T E X C O O R D 3 ; 
 	 	 U N I T Y _ F O G _ C O O R D S ( 4 ) 
 	 } ; 
 	 
 	 s t r u c t   v 2 f _ s i m p l e 
 	 { 
 	 	 f l o a t 4   p o s   :   S V _ P O S I T I O N ; 
 	 	 f l o a t 4   v i e w I n t e r p o l a t o r   :   T E X C O O R D 0 ; 
 	 	 f l o a t 4   b u m p C o o r d s   :   T E X C O O R D 1 ; 
 	 	 U N I T Y _ F O G _ C O O R D S ( 2 ) 
 	 } ; 
 
 	 / /   t e x t u r e s 
 	 s a m p l e r 2 D   _ B u m p M a p ; 
 	 s a m p l e r 2 D   _ R e f l e c t i o n T e x ; 
 	 s a m p l e r 2 D   _ R e f r a c t i o n T e x ; 
 	 s a m p l e r 2 D   _ S h o r e T e x ; 
 	 s a m p l e r 2 D _ f l o a t   _ C a m e r a D e p t h T e x t u r e ; 
 
 	 / /   c o l o r s   i n   u s e 
 	 u n i f o r m   f l o a t 4   _ R e f r C o l o r D e p t h ; 
 	 u n i f o r m   f l o a t 4   _ S p e c u l a r C o l o r ; 
 	 u n i f o r m   f l o a t 4   _ B a s e C o l o r ; 
 	 u n i f o r m   f l o a t 4   _ R e f l e c t i o n C o l o r ; 
 	 
 	 / /   e d g e   &   s h o r e   f a d i n g 
 	 u n i f o r m   f l o a t 4   _ I n v F a d e P a r e m e t e r ; 
 
 	 / /   s p e c u l a r i t y 
 	 u n i f o r m   f l o a t   _ S h i n i n e s s ; 
 	 u n i f o r m   f l o a t 4   _ W o r l d L i g h t D i r ; 
 
 	 / /   f r e s n e l ,   v e r t e x   &   b u m p   d i s p l a c e m e n t s   &   s t r e n g t h 
 	 u n i f o r m   f l o a t 4   _ D i s t o r t P a r a m s ; 
 	 u n i f o r m   f l o a t   _ F r e s n e l S c a l e ; 
 	 u n i f o r m   f l o a t 4   _ B u m p T i l i n g ; 
 	 u n i f o r m   f l o a t 4   _ B u m p D i r e c t i o n ; 
 
 	 u n i f o r m   f l o a t 4   _ G A m p l i t u d e ; 
 	 u n i f o r m   f l o a t 4   _ G F r e q u e n c y ; 
 	 u n i f o r m   f l o a t 4   _ G S t e e p n e s s ; 
 	 u n i f o r m   f l o a t 4   _ G S p e e d ; 
 	 u n i f o r m   f l o a t 4   _ G D i r e c t i o n A B ; 
 	 u n i f o r m   f l o a t 4   _ G D i r e c t i o n C D ; 
 	 
 	 / /   f o a m 
 	 u n i f o r m   f l o a t 4   _ F o a m ; 
 	 
 	 / /   s h o r t c u t s 
 	 # d e f i n e   P E R _ P I X E L _ D I S P L A C E   _ D i s t o r t P a r a m s . x 
 	 # d e f i n e   R E A L T I M E _ D I S T O R T I O N   _ D i s t o r t P a r a m s . y 
 	 # d e f i n e   F R E S N E L _ P O W E R   _ D i s t o r t P a r a m s . z 
 	 # d e f i n e   V E R T E X _ W O R L D _ N O R M A L   i . n o r m a l I n t e r p o l a t o r . x y z 
 	 # d e f i n e   F R E S N E L _ B I A S   _ D i s t o r t P a r a m s . w 
 	 # d e f i n e   N O R M A L _ D I S P L A C E M E N T _ P E R _ V E R T E X   _ I n v F a d e P a r e m e t e r . z 
 	 
 	 / / 
 	 / /   H Q   V E R S I O N 
 	 / / 
 	 
 	 v 2 f   v e r t ( a p p d a t a _ f u l l   v ) 
 	 { 
 	 	 v 2 f   o ; 
 	 	 
 	 	 h a l f 3   w o r l d S p a c e V e r t e x   =   m u l ( u n i t y _ O b j e c t T o W o r l d , ( v . v e r t e x ) ) . x y z ; 
 	 	 h a l f 3   v t x F o r A n i   =   ( w o r l d S p a c e V e r t e x ) . x z z ; 
 
 	 	 h a l f 3   n r m l ; 
 	 	 h a l f 3   o f f s e t s ; 
 	 	 G e r s t n e r   ( 
 	 	 	 o f f s e t s ,   n r m l ,   v . v e r t e x . x y z ,   v t x F o r A n i , 	 	 	 	 	 	 / /   o f f s e t s ,   n r m l   w i l l   b e   w r i t t e n 
 	 	 	 _ G A m p l i t u d e , 	 	 	 	 	 	 	 	 	 	 	 	 / /   a m p l i t u d e 
 	 	 	 _ G F r e q u e n c y , 	 	 	 	 	 	 	 	 	 	 	 	 / /   f r e q u e n c y 
 	 	 	 _ G S t e e p n e s s , 	 	 	 	 	 	 	 	 	 	 	 	 / /   s t e e p n e s s 
 	 	 	 _ G S p e e d , 	 	 	 	 	 	 	 	 	 	 	 	 	 / /   s p e e d 
 	 	 	 _ G D i r e c t i o n A B , 	 	 	 	 	 	 	 	 	 	 	 	 / /   d i r e c t i o n   #   1 ,   2 
 	 	 	 _ G D i r e c t i o n C D 	 	 	 	 	 	 	 	 	 	 	 	 / /   d i r e c t i o n   #   3 ,   4 
 	 	 ) ; 
 	 	 
 	 	 v . v e r t e x . x y z   + =   o f f s e t s ; 
 	 	 
 	 	 / /   o n e   c a n   a l s o   u s e   w o r l d S p a c e V e r t e x . x z   h e r e   ( s p e e d ! ) ,   a l b e i t   i t ' l l   e n d   u p   a   l i t t l e   s k e w e d 
 	 	 h a l f 2   t i l e a b l e U v   =   m u l ( u n i t y _ O b j e c t T o W o r l d , ( v . v e r t e x ) ) . x z ; 
 	 	 
 	 	 o . b u m p C o o r d s . x y z w   =   ( t i l e a b l e U v . x y x y   +   _ T i m e . x x x x   *   _ B u m p D i r e c t i o n . x y z w )   *   _ B u m p T i l i n g . x y z w ; 
 
 	 	 o . v i e w I n t e r p o l a t o r . x y z   =   w o r l d S p a c e V e r t e x   -   _ W o r l d S p a c e C a m e r a P o s ; 
 
 	 	 o . p o s   =   m u l ( U N I T Y _ M A T R I X _ M V P ,   v . v e r t e x ) ; 
 
 	 	 C o m p u t e S c r e e n A n d G r a b P a s s P o s ( o . p o s ,   o . s c r e e n P o s ,   o . g r a b P a s s P o s ) ; 
 	 	 
 	 	 o . n o r m a l I n t e r p o l a t o r . x y z   =   n r m l ; 
 	 	 
 	 	 o . v i e w I n t e r p o l a t o r . w   =   s a t u r a t e ( o f f s e t s . y ) ; 
 	 	 o . n o r m a l I n t e r p o l a t o r . w   =   1 ; / / G e t D i s t a n c e F a d e o u t ( o . s c r e e n P o s . w ,   D I S T A N C E _ S C A L E ) ; 
 	 	 
 	 	 U N I T Y _ T R A N S F E R _ F O G ( o , o . p o s ) ; 
 	 	 r e t u r n   o ; 
 	 } 
 
 	 h a l f 4   f r a g (   v 2 f   i   )   :   S V _ T a r g e t 
 	 { 
 	 	 h a l f 3   w o r l d N o r m a l   =   P e r P i x e l N o r m a l ( _ B u m p M a p ,   i . b u m p C o o r d s ,   V E R T E X _ W O R L D _ N O R M A L ,   P E R _ P I X E L _ D I S P L A C E ) ; 
 	 	 h a l f 3   v i e w V e c t o r   =   n o r m a l i z e ( i . v i e w I n t e r p o l a t o r . x y z ) ; 
 
 	 	 h a l f 4   d i s t o r t O f f s e t   =   h a l f 4 ( w o r l d N o r m a l . x z   *   R E A L T I M E _ D I S T O R T I O N   *   1 0 . 0 ,   0 ,   0 ) ; 
 	 	 h a l f 4   s c r e e n W i t h O f f s e t   =   i . s c r e e n P o s   +   d i s t o r t O f f s e t ; 
 	 	 h a l f 4   g r a b W i t h O f f s e t   =   i . g r a b P a s s P o s   +   d i s t o r t O f f s e t ; 
 	 	 
 	 	 h a l f 4   r t R e f r a c t i o n s N o D i s t o r t   =   t e x 2 D p r o j ( _ R e f r a c t i o n T e x ,   U N I T Y _ P R O J _ C O O R D ( i . g r a b P a s s P o s ) ) ; 
 	 	 h a l f   r e f r F i x   =   S A M P L E _ D E P T H _ T E X T U R E _ P R O J ( _ C a m e r a D e p t h T e x t u r e ,   U N I T Y _ P R O J _ C O O R D ( g r a b W i t h O f f s e t ) ) ; 
 	 	 h a l f 4   r t R e f r a c t i o n s   =   t e x 2 D p r o j ( _ R e f r a c t i o n T e x ,   U N I T Y _ P R O J _ C O O R D ( g r a b W i t h O f f s e t ) ) ; 
 	 	 
 	 	 # i f d e f   W A T E R _ R E F L E C T I V E 
 	 	 	 h a l f 4   r t R e f l e c t i o n s   =   t e x 2 D p r o j ( _ R e f l e c t i o n T e x ,   U N I T Y _ P R O J _ C O O R D ( s c r e e n W i t h O f f s e t ) ) ; 
 	 	 # e n d i f 
 
 	 	 # i f d e f   W A T E R _ E D G E B L E N D _ O N 
 	 	 i f   ( L i n e a r E y e D e p t h ( r e f r F i x )   <   i . s c r e e n P o s . z ) 
 	 	 	 r t R e f r a c t i o n s   =   r t R e f r a c t i o n s N o D i s t o r t ; 
 	 	 # e n d i f 
 	 	 
 	 	 h a l f 3   r e f l e c t V e c t o r   =   n o r m a l i z e ( r e f l e c t ( v i e w V e c t o r ,   w o r l d N o r m a l ) ) ; 
 	 	 h a l f 3   h   =   n o r m a l i z e   ( ( _ W o r l d L i g h t D i r . x y z )   +   v i e w V e c t o r . x y z ) ; 
 	 	 f l o a t   n h   =   m a x   ( 0 ,   d o t   ( w o r l d N o r m a l ,   - h ) ) ; 
 	 	 f l o a t   s p e c   =   m a x ( 0 . 0 , p o w   ( n h ,   _ S h i n i n e s s ) ) ; 
 	 	 
 	 	 h a l f 4   e d g e B l e n d F a c t o r s   =   h a l f 4 ( 1 . 0 ,   0 . 0 ,   0 . 0 ,   0 . 0 ) ; 
 	 	 
 	 	 # i f d e f   W A T E R _ E D G E B L E N D _ O N 
 	 	 	 h a l f   d e p t h   =   S A M P L E _ D E P T H _ T E X T U R E _ P R O J ( _ C a m e r a D e p t h T e x t u r e ,   U N I T Y _ P R O J _ C O O R D ( i . s c r e e n P o s ) ) ; 
 	 	 	 d e p t h   =   L i n e a r E y e D e p t h ( d e p t h ) ; 
 	 	 	 e d g e B l e n d F a c t o r s   =   s a t u r a t e ( _ I n v F a d e P a r e m e t e r   *   ( d e p t h - i . s c r e e n P o s . w ) ) ; 
 	 	 	 e d g e B l e n d F a c t o r s . y   =   1 . 0 - e d g e B l e n d F a c t o r s . y ; 
 	 	 # e n d i f 
 	 	 
 	 	 / /   s h a d i n g   f o r   f r e s n e l   t e r m 
 	 	 w o r l d N o r m a l . x z   * =   _ F r e s n e l S c a l e ; 
 	 	 h a l f   r e f l 2 R e f r   =   F r e s n e l ( v i e w V e c t o r ,   w o r l d N o r m a l ,   F R E S N E L _ B I A S ,   F R E S N E L _ P O W E R ) ; 
 	 	 
 	 	 / /   b a s e ,   d e p t h   &   r e f l e c t i o n   c o l o r s 
 	 	 h a l f 4   b a s e C o l o r   =   E x t i n c t C o l o r   ( _ B a s e C o l o r ,   i . v i e w I n t e r p o l a t o r . w   *   _ I n v F a d e P a r e m e t e r . w ) ; 
 	 	 # i f d e f   W A T E R _ R E F L E C T I V E 
 	 	 	 h a l f 4   r e f l e c t i o n C o l o r   =   l e r p   ( r t R e f l e c t i o n s , _ R e f l e c t i o n C o l o r , _ R e f l e c t i o n C o l o r . a ) ; 
 	 	 # e l s e 
 	 	 	 h a l f 4   r e f l e c t i o n C o l o r   =   _ R e f l e c t i o n C o l o r ; 
 	 	 # e n d i f 
 	 	 
 	 	 b a s e C o l o r   =   l e r p   ( l e r p   ( r t R e f r a c t i o n s ,   b a s e C o l o r ,   b a s e C o l o r . a ) ,   r e f l e c t i o n C o l o r ,   r e f l 2 R e f r ) ; 
 	 	 b a s e C o l o r   =   b a s e C o l o r   +   s p e c   *   _ S p e c u l a r C o l o r ; 
 	 	 
 	 	 / /   h a n d l e   f o a m 
 	 	 h a l f 4   f o a m   =   F o a m ( _ S h o r e T e x ,   i . b u m p C o o r d s   *   2 . 0 ) ; 
 	 	 b a s e C o l o r . r g b   + =   f o a m . r g b   *   _ F o a m . x   *   ( e d g e B l e n d F a c t o r s . y   +   s a t u r a t e ( i . v i e w I n t e r p o l a t o r . w   -   _ F o a m . y ) ) ; 
 	 	 
 	 	 b a s e C o l o r . a   =   e d g e B l e n d F a c t o r s . x ; 
 	 	 U N I T Y _ A P P L Y _ F O G ( i . f o g C o o r d ,   b a s e C o l o r ) ; 
 	 	 r e t u r n   b a s e C o l o r ; 
 	 } 
 	 
 	 / / 
 	 / /   M Q   V E R S I O N 
 	 / / 
 	 
 	 v 2 f _ n o G r a b   v e r t 3 0 0 ( a p p d a t a _ f u l l   v ) 
 	 { 
 	 	 v 2 f _ n o G r a b   o ; 
 	 	 
 	 	 h a l f 3   w o r l d S p a c e V e r t e x   =   m u l ( u n i t y _ O b j e c t T o W o r l d , ( v . v e r t e x ) ) . x y z ; 
 	 	 h a l f 3   v t x F o r A n i   =   ( w o r l d S p a c e V e r t e x ) . x z z ; 
 
 	 	 h a l f 3   n r m l ; 
 	 	 h a l f 3   o f f s e t s ; 
 	 	 G e r s t n e r   ( 
 	 	 	 o f f s e t s ,   n r m l ,   v . v e r t e x . x y z ,   v t x F o r A n i , 	 	 	 	 	 	 / /   o f f s e t s ,   n r m l   w i l l   b e   w r i t t e n 
 	 	 	 _ G A m p l i t u d e , 	 	 	 	 	 	 	 	 	 	 	 	 / /   a m p l i t u d e 
 	 	 	 _ G F r e q u e n c y , 	 	 	 	 	 	 	 	 	 	 	 	 / /   f r e q u e n c y 
 	 	 	 _ G S t e e p n e s s , 	 	 	 	 	 	 	 	 	 	 	 	 / /   s t e e p n e s s 
 	 	 	 _ G S p e e d , 	 	 	 	 	 	 	 	 	 	 	 	 	 / /   s p e e d 
 	 	 	 _ G D i r e c t i o n A B , 	 	 	 	 	 	 	 	 	 	 	 	 / /   d i r e c t i o n   #   1 ,   2 
 	 	 	 _ G D i r e c t i o n C D 	 	 	 	 	 	 	 	 	 	 	 	 / /   d i r e c t i o n   #   3 ,   4 
 	 	 ) ; 
 	 	 
 	 	 v . v e r t e x . x y z   + =   o f f s e t s ; 
 	 	 
 	 	 / /   o n e   c a n   a l s o   u s e   w o r l d S p a c e V e r t e x . x z   h e r e   ( s p e e d ! ) ,   a l b e i t   i t ' l l   e n d   u p   a   l i t t l e   s k e w e d 
 	 	 h a l f 2   t i l e a b l e U v   =   m u l ( u n i t y _ O b j e c t T o W o r l d , v . v e r t e x ) . x z ; 
 	 	 o . b u m p C o o r d s . x y z w   =   ( t i l e a b l e U v . x y x y   +   _ T i m e . x x x x   *   _ B u m p D i r e c t i o n . x y z w )   *   _ B u m p T i l i n g . x y z w ; 
 
 	 	 o . v i e w I n t e r p o l a t o r . x y z   =   w o r l d S p a c e V e r t e x   -   _ W o r l d S p a c e C a m e r a P o s ; 
 
 	 	 o . p o s   =   m u l ( U N I T Y _ M A T R I X _ M V P ,   v . v e r t e x ) ; 
 
 	 	 o . s c r e e n P o s   =   C o m p u t e S c r e e n P o s ( o . p o s ) ; 
 	 	 
 	 	 o . n o r m a l I n t e r p o l a t o r . x y z   =   n r m l ; 
 	 	 o . n o r m a l I n t e r p o l a t o r . w   =   1 ; / / G e t D i s t a n c e F a d e o u t ( o . s c r e e n P o s . w ,   D I S T A N C E _ S C A L E ) ; 
 	 	 
 	 	 U N I T Y _ T R A N S F E R _ F O G ( o , o . p o s ) ; 
 	 	 r e t u r n   o ; 
 	 } 
 
 	 h a l f 4   f r a g 3 0 0 (   v 2 f _ n o G r a b   i   )   :   S V _ T a r g e t 
 	 { 
 	 	 h a l f 3   w o r l d N o r m a l   =   P e r P i x e l N o r m a l ( _ B u m p M a p ,   i . b u m p C o o r d s ,   n o r m a l i z e ( V E R T E X _ W O R L D _ N O R M A L ) ,   P E R _ P I X E L _ D I S P L A C E ) ; 
 
 	 	 h a l f 3   v i e w V e c t o r   =   n o r m a l i z e ( i . v i e w I n t e r p o l a t o r . x y z ) ; 
 
 	 	 h a l f 4   d i s t o r t O f f s e t   =   h a l f 4 ( w o r l d N o r m a l . x z   *   R E A L T I M E _ D I S T O R T I O N   *   1 0 . 0 ,   0 ,   0 ) ; 
 	 	 h a l f 4   s c r e e n W i t h O f f s e t   =   i . s c r e e n P o s   +   d i s t o r t O f f s e t ; 
 	 	 
 	 	 # i f d e f   W A T E R _ R E F L E C T I V E 
 	 	 	 h a l f 4   r t R e f l e c t i o n s   =   t e x 2 D p r o j ( _ R e f l e c t i o n T e x ,   U N I T Y _ P R O J _ C O O R D ( s c r e e n W i t h O f f s e t ) ) ; 
 	 	 # e n d i f 
 	 	 
 	 	 h a l f 3   r e f l e c t V e c t o r   =   n o r m a l i z e ( r e f l e c t ( v i e w V e c t o r ,   w o r l d N o r m a l ) ) ; 
 	 	 h a l f 3   h   =   n o r m a l i z e   ( _ W o r l d L i g h t D i r . x y z   +   v i e w V e c t o r . x y z ) ; 
 	 	 f l o a t   n h   =   m a x   ( 0 ,   d o t   ( w o r l d N o r m a l ,   - h ) ) ; 
 	 	 f l o a t   s p e c   =   m a x ( 0 . 0 , p o w   ( n h ,   _ S h i n i n e s s ) ) ; 
 	 	 
 	 	 h a l f 4   e d g e B l e n d F a c t o r s   =   h a l f 4 ( 1 . 0 ,   0 . 0 ,   0 . 0 ,   0 . 0 ) ; 
 	 	 
 	 	 # i f d e f   W A T E R _ E D G E B L E N D _ O N 
 	 	 	 h a l f   d e p t h   =   S A M P L E _ D E P T H _ T E X T U R E _ P R O J ( _ C a m e r a D e p t h T e x t u r e ,   U N I T Y _ P R O J _ C O O R D ( i . s c r e e n P o s ) ) ; 
 	 	 	 d e p t h   =   L i n e a r E y e D e p t h ( d e p t h ) ; 
 	 	 	 e d g e B l e n d F a c t o r s   =   s a t u r a t e ( _ I n v F a d e P a r e m e t e r   *   ( d e p t h - i . s c r e e n P o s . z ) ) ; 
 	 	 	 e d g e B l e n d F a c t o r s . y   =   1 . 0 - e d g e B l e n d F a c t o r s . y ; 
 	 	 # e n d i f 
 	 	 
 	 	 w o r l d N o r m a l . x z   * =   _ F r e s n e l S c a l e ; 
 	 	 h a l f   r e f l 2 R e f r   =   F r e s n e l ( v i e w V e c t o r ,   w o r l d N o r m a l ,   F R E S N E L _ B I A S ,   F R E S N E L _ P O W E R ) ; 
 	 	 
 	 	 h a l f 4   b a s e C o l o r   =   _ B a s e C o l o r ; 
 	 	 # i f d e f   W A T E R _ R E F L E C T I V E 
 	 	 	 b a s e C o l o r   =   l e r p   ( b a s e C o l o r ,   l e r p   ( r t R e f l e c t i o n s , _ R e f l e c t i o n C o l o r , _ R e f l e c t i o n C o l o r . a ) ,   s a t u r a t e ( r e f l 2 R e f r   *   2 . 0 ) ) ; 
 	 	 # e l s e 
 	 	 	 b a s e C o l o r   =   l e r p   ( b a s e C o l o r ,   _ R e f l e c t i o n C o l o r ,   s a t u r a t e ( r e f l 2 R e f r   *   2 . 0 ) ) ; 
 	 	 # e n d i f 
 	 	 
 	 	 b a s e C o l o r   =   b a s e C o l o r   +   s p e c   *   _ S p e c u l a r C o l o r ; 
 	 	 
 	 	 b a s e C o l o r . a   =   e d g e B l e n d F a c t o r s . x   *   s a t u r a t e ( 0 . 5   +   r e f l 2 R e f r   *   1 . 0 ) ; 
 	 	 U N I T Y _ A P P L Y _ F O G ( i . f o g C o o r d ,   b a s e C o l o r ) ; 
 	 	 r e t u r n   b a s e C o l o r ; 
 	 } 
 	 
 	 / / 
 	 / /   L Q   V E R S I O N 
 	 / / 
 	 
 	 v 2 f _ s i m p l e   v e r t 2 0 0 ( a p p d a t a _ f u l l   v ) 
 	 { 
 	 	 v 2 f _ s i m p l e   o ; 
 	 	 
 	 	 h a l f 3   w o r l d S p a c e V e r t e x   =   m u l ( u n i t y _ O b j e c t T o W o r l d ,   v . v e r t e x ) . x y z ; 
 	 	 h a l f 2   t i l e a b l e U v   =   w o r l d S p a c e V e r t e x . x z ; 
 
 	 	 o . b u m p C o o r d s . x y z w   =   ( t i l e a b l e U v . x y x y   +   _ T i m e . x x x x   *   _ B u m p D i r e c t i o n . x y z w )   *   _ B u m p T i l i n g . x y z w ; 
 
 	 	 o . v i e w I n t e r p o l a t o r . x y z   =   w o r l d S p a c e V e r t e x - _ W o r l d S p a c e C a m e r a P o s ; 
 	 	 
 	 	 o . p o s   =   m u l ( U N I T Y _ M A T R I X _ M V P ,     v . v e r t e x ) ; 
 	 	 
 	 	 o . v i e w I n t e r p o l a t o r . w   =   1 ; / / G e t D i s t a n c e F a d e o u t ( C o m p u t e S c r e e n P o s ( o . p o s ) . w ,   D I S T A N C E _ S C A L E ) ; 
 	 	 
 	 	 U N I T Y _ T R A N S F E R _ F O G ( o , o . p o s ) ; 
 	 	 r e t u r n   o ; 
 
 	 } 
 
 	 h a l f 4   f r a g 2 0 0 (   v 2 f _ s i m p l e   i   )   :   S V _ T a r g e t 
 	 { 
 	 	 h a l f 3   w o r l d N o r m a l   =   P e r P i x e l N o r m a l ( _ B u m p M a p ,   i . b u m p C o o r d s ,   h a l f 3 ( 0 , 1 , 0 ) ,   P E R _ P I X E L _ D I S P L A C E ) ; 
 	 	 h a l f 3   v i e w V e c t o r   =   n o r m a l i z e ( i . v i e w I n t e r p o l a t o r . x y z ) ; 
 
 	 	 h a l f 3   r e f l e c t V e c t o r   =   n o r m a l i z e ( r e f l e c t ( v i e w V e c t o r ,   w o r l d N o r m a l ) ) ; 
 	 	 h a l f 3   h   =   n o r m a l i z e   ( ( _ W o r l d L i g h t D i r . x y z )   +   v i e w V e c t o r . x y z ) ; 
 	 	 f l o a t   n h   =   m a x   ( 0 ,   d o t   ( w o r l d N o r m a l ,   - h ) ) ; 
 	 	 f l o a t   s p e c   =   m a x ( 0 . 0 , p o w   ( n h ,   _ S h i n i n e s s ) ) ; 
 
 	 	 w o r l d N o r m a l . x z   * =   _ F r e s n e l S c a l e ; 
 	 	 h a l f   r e f l 2 R e f r   =   F r e s n e l ( v i e w V e c t o r ,   w o r l d N o r m a l ,   F R E S N E L _ B I A S ,   F R E S N E L _ P O W E R ) ; 
 
 	 	 h a l f 4   b a s e C o l o r   =   _ B a s e C o l o r ; 
 	 	 b a s e C o l o r   =   l e r p ( b a s e C o l o r ,   _ R e f l e c t i o n C o l o r ,   s a t u r a t e ( r e f l 2 R e f r   *   2 . 0 ) ) ; 
 	 	 b a s e C o l o r . a   =   s a t u r a t e ( 2 . 0   *   r e f l 2 R e f r   +   0 . 5 ) ; 
 
 	 	 b a s e C o l o r . r g b   + =   s p e c   *   _ S p e c u l a r C o l o r . r g b ; 
 	 	 U N I T Y _ A P P L Y _ F O G ( i . f o g C o o r d ,   b a s e C o l o r ) ; 
 	 	 r e t u r n   b a s e C o l o r ; 
 	 } 
 	 
 E N D C G 
 
 S u b s h a d e r 
 { 
 	 T a g s   { " R e n d e r T y p e " = " T r a n s p a r e n t "   " Q u e u e " = " T r a n s p a r e n t "   " W a t e r " = " W a t e r 4 A d v a n c e d " } 
 	 
 	 L o d   5 0 0 
 	 C o l o r M a s k   R G B 
 	 
 	 G r a b P a s s   {   " _ R e f r a c t i o n T e x "   } 
 	 
 	 P a s s   { 
 	 	 	 B l e n d   S r c A l p h a   O n e M i n u s S r c A l p h a 
 	 	 	 Z T e s t   L E q u a l 
 	 	 	 Z W r i t e   O f f 
 	 	 	 C u l l   O f f 
 	 	 
 	 	 	 C G P R O G R A M 
 	 	 
 	 	 	 # p r a g m a   t a r g e t   3 . 0 
 	 	 
 	 	 	 # p r a g m a   v e r t e x   v e r t 
 	 	 	 # p r a g m a   f r a g m e n t   f r a g 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e _ f o g 
 	 	 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ V E R T E X _ D I S P L A C E M E N T _ O N   W A T E R _ V E R T E X _ D I S P L A C E M E N T _ O F F 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ E D G E B L E N D _ O N   W A T E R _ E D G E B L E N D _ O F F 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ R E F L E C T I V E   W A T E R _ S I M P L E 
 	 	 
 	 	 	 E N D C G 
 	 } 
 } 
 
 S u b s h a d e r 
 { 
 	 T a g s   { " R e n d e r T y p e " = " T r a n s p a r e n t "   " Q u e u e " = " T r a n s p a r e n t "   " W a t e r " = " W a t e r 4 A d v a n c e d " } 
 	 
 	 L o d   3 0 0 
 	 C o l o r M a s k   R G B 
 	 
 	 P a s s   { 
 	 	 	 B l e n d   S r c A l p h a   O n e M i n u s S r c A l p h a 
 	 	 	 Z T e s t   L E q u a l 
 	 	 	 Z W r i t e   O f f 
 	 	 	 C u l l   O f f 
 	 	 
 	 	 	 C G P R O G R A M 
 	 	 
 	 	 	 # p r a g m a   t a r g e t   3 . 0 
 	 	 
 	 	 	 # p r a g m a   v e r t e x   v e r t 3 0 0 
 	 	 	 # p r a g m a   f r a g m e n t   f r a g 3 0 0 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e _ f o g 
 	 	 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ V E R T E X _ D I S P L A C E M E N T _ O N   W A T E R _ V E R T E X _ D I S P L A C E M E N T _ O F F 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ E D G E B L E N D _ O N   W A T E R _ E D G E B L E N D _ O F F 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e   W A T E R _ R E F L E C T I V E   W A T E R _ S I M P L E 
 	 	 
 	 	 	 E N D C G 
 	 } 
 } 
 
 S u b s h a d e r 
 { 
 	 T a g s   { " R e n d e r T y p e " = " T r a n s p a r e n t "   " Q u e u e " = " T r a n s p a r e n t "   " W a t e r " = " W a t e r 4 A d v a n c e d " } 
 	 
 	 L o d   2 0 0 
 	 C o l o r M a s k   R G B 
 	 
 	 P a s s   { 
 	 	 	 B l e n d   S r c A l p h a   O n e M i n u s S r c A l p h a 
 	 	 	 Z T e s t   L E q u a l 
 	 	 	 Z W r i t e   O f f 
 	 	 	 C u l l   O f f 
 	 	 
 	 	 	 C G P R O G R A M 
 	 	 
 	 	 	 # p r a g m a   v e r t e x   v e r t 2 0 0 
 	 	 	 # p r a g m a   f r a g m e n t   f r a g 2 0 0 
 	 	 	 # p r a g m a   m u l t i _ c o m p i l e _ f o g 
 	 	 
 	 	 	 E N D C G 
 	 } 
 } 
 
 F a l l b a c k   " T r a n s p a r e n t / D i f f u s e " 
 } 
 