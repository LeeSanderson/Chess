/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

//#pragma once
//#include <iostream>
//class ChessLogStream{
//
//};
//
//template <class T>
//ChessLogStream& operator<<(ChessLogStream& s, T o){
//	*GetChessOutputStream() << o;
//	return s;
//}
//
//extern ChessLogStream theChessLogStream;
//
////ChessLogStream& ChessLog(){ return theChessLogStream;}
//inline std::ostream& ChessLog(){ return *GetChessOutputStream();}

const int EXECUTION=0;
const int SCHEDULE=0;
const int ENABLED_SET=0;
//#define ENABLE_CHESS_LOG 
template<class A, class B> void CHESS_LOG(const A& a, const B& b) { 
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << std::endl; 
#endif
}
template<class A, class B, class C> void CHESS_LOG(const A& a, const B& b, const C& c){ 
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << ' ' << c << ' ' << std::endl; 
#endif
}
template<class A, class B, class C, class D> void CHESS_LOG(const A& a, const B& b, const C& c, const D& d){ 
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << ' ' << c << ' ' << d << ' '<< std::endl; 
#endif
}
template<class A, class B, class C, class D, class E> void CHESS_LOG(const A& a, const B& b, const C& c, const D& d, const E& e){ 
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << ' ' << c << ' ' << d << ' ' << e << ' ' << std::endl; 
#endif
}
template<class A, class B, class C, class D, class E, class F> void CHESS_LOG(const A& a, const B& b, const C& c, const D& d, const E& e, const F& f){ 
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << ' ' << c << ' ' << d << ' ' << e << ' ' << f << ' ' << std::endl; 
#endif
}
template<class A, class B, class C, class D, class E, class F, class G> void CHESS_LOG(const A& a, const B& b, const C& c, const D& d, const E& e, const F& f, const G& g){
#ifdef ENABLE_CHESS_LOG 
	*GetChessOutputStream() << b << ' ' << c << ' ' << d << ' ' << e << ' ' << f << ' ' << g << ' '<< std::endl; 
#endif
}

