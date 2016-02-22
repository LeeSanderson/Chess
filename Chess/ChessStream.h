/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#pragma once

#include "ChessApi.h"
#include <ostream>

CHESS_API std::ostream* GetChessErrorStream();
CHESS_API std::ostream* GetChessOutputStream();

