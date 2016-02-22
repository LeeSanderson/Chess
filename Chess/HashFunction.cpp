// Using a perfect hash function and a Mersenne Twister implementation to 
// as the random hash table.

unsigned long randomHash[256];
int randomHashInitialized = 0;

unsigned long MT[624];
int mersenneIndex = 0;

void InitializeMersenneTable(unsigned long seed){
	int i;
	MT[0] = seed;
	for(i = 1; i<624; i++){
		MT[i] = i + 1812433253 * (MT[i-1] ^ (MT[i-1]>>30));
	}
}

void GenerateMersenneTable(){
	int i;
	for(i=0; i<624; i++){
		unsigned long y = (MT[i] >> 31) + (MT[(i+1)%624] & 0x7fffffff);
		MT[i] = MT[(i + 397)%624] ^ (y >> 1);
		if(y%2){
			MT[i] = MT[i] ^ 0x9908b0df;
		}
	}
}

unsigned long NextMersenneNumber(){
	unsigned long y;
	if(mersenneIndex == 0)
		GenerateMersenneTable();
	y = MT[mersenneIndex];
	y = y ^ (y >> 11);
	y = y ^ ((y << 7) & 0x9d2c5680);
	y = y ^ ((y << 15) & 0xefc60000);
	y = y ^ (y >> 18);
	mersenneIndex++;
	if(mersenneIndex == 624)
		mersenneIndex = 0;
	return y;
}

void InitializeRandomHash(){
	int i;
	InitializeMersenneTable(0xdeadbeaf);
	for(i=0; i<100; i++)
		GenerateMersenneTable();
	for(i=0; i<256; i++){
		randomHash[i] = NextMersenneNumber();
	}
	randomHashInitialized = 1;
}
