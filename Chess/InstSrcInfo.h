/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include <iostream>
#include <string>
#include <strstream>

class InstSrcInfo{
public:
	int id;
	std::string filename;
	int lineno;
};

inline std::ostream& operator<<(std::ostream& os, InstSrcInfo& info){
	os << info.id << "|" << info.filename << "|" << info.lineno;
	return os;
}

inline std::istream& operator>>(std::istream& is, InstSrcInfo& info){
    char ch;
	is >> info.id;
    is >> ch;
    getline(is, info.filename, '|');
    //is >> ch;
	is >> info.lineno;
	return is;
}