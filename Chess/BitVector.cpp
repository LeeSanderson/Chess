/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "BitVector.h"
/*#include <set>

class LargeBV{
private:
	// set or vector depends on how sparse the bit vector is going to be
	std::set<size_t> bv;
public:
	LargeBV(size_t smallbv){
		BitVector s(smallbv);
		for(size_t i=0; i<BitVector::INTEGER_SIZE-1; i++){
			if(s.Get(i))
				bv.insert(i);
		}
	}
	LargeBV(const LargeBV* other){
		bv = other->bv;
	}

	void Set(size_t index, bool value){
		if(value) 
			bv.insert(index);
		else
			bv.erase(index);
	}

	bool Get(size_t index){
		return bv.find(index) != bv.end();
	}

	bool IsCompressible() const {
		if(bv.size() == 0) return true;
		std::set<size_t>::const_iterator max = (--bv.end());
		return *max < BitVector::INTEGER_SIZE-1;
	}

	size_t Compress(){
		assert(IsCompressible());
		BitVector smallbv;
		for(std::set<size_t>::iterator i=bv.begin(); i!=bv.end(); i++){
			smallbv.Set(*i, true);
		}
		assert(!smallbv.IsLargeBV());
		return smallbv.smallbv;
	}

	bool IsEmpty() const{
		return bv.empty();
	}

	size_t FindIndexLargerThan(size_t index) const{
		assert(!bv.empty());
		std::set<size_t>::const_iterator r = bv.upper_bound(index);
		if(r != bv.end()){
			return *r;
		}
		if(bv.find(0) != bv.end())
			return 0;
		r = bv.upper_bound(0);
		return *r;
	}

};

void BitVector::LargeBVSet(size_t index, bool value){
	if(IsLargeBV()){
		LargeBV* largebv = ((LargeBV*)smallbv);
		largebv->Set(index, value);
		if(!value && largebv->IsCompressible()){
			// convert largebv to smallbv
			smallbv = largebv->Compress();
			assert(!IsLargeBV());
			delete largebv;
		}
	}
	else{
		// Convert smallbv to a LargeBV;
		assert(index >= INTEGER_SIZE-1);
		LargeBV* largebv = new LargeBV(smallbv);
		largebv->Set(index, value);
		smallbv = (size_t)(largebv);
		assert(IsLargeBV());
	}
}

bool BitVector::LargeBVGet(size_t index)const{
	assert(IsLargeBV());
	LargeBV* largebv = ((LargeBV*)smallbv);
	return largebv->Get(index);
}

void BitVector::LargeBVIntersect(const BitVector& v) {
	LargeBV* thisBv;
	LargeBV* otherBv;
	if(IsLargeBV()){
		thisBv = ((LargeBV*)smallbv);
	}else{
		thisBv = new LargeBV(smallbv);
	}

	if(v.IsLargeBV()){
		otherBv = ((LargeBV*)v.smallbv);
	}
	else{
		otherBv = new LargeBV(v.smallbv);
	}

	LargeBV* resultBv = new LargeBV(0);
	std::insert_iterator<std::set<SyncVar> > ii(resultBv->bv, resultBv->bv.begin());
	set_intersection(thisBv->bv.begin(), thisBv->bv.end(), otherBv->bv.begin(), otherBv->bv.end(), ii);

	if(!IsLargeBV())
		delete thisBv;
	if(!v.IsLargeBV())
		delete otherBv;

	if(resultBv->IsCompressible()){
		smallbv = resultBv->Compress();
		assert(!IsLargeBV());
		delete resultBv;
		assert(!IsLargeBV());
	}
	else{
		smallbv = (size_t)resultBv;
		assert(IsLargeBV());
	}
}
void BitVector::LargeBVUnion(const BitVector& v) {
	assert(!"NotImplemented");
	//BitVector(0);
	smallbv = 0;
}

void BitVector::LargeBVCopy(BitVector& res)const{
	assert(IsLargeBV());
	const LargeBV* largebv = ((LargeBV*)smallbv);
	res.smallbv = (size_t)new LargeBV(largebv);
}

void BitVector::LargeBVDestruct(){
	assert(IsLargeBV());
	LargeBV* largebv = ((LargeBV*)smallbv);
	delete largebv;
	smallbv=1;
}

bool BitVector::LargeBVEquals(const BitVector& other) const{
	assert(!"NotImplemented");
	((BitVector*)(this))->smallbv = 0; // makes prefix happy
	return false;
}

bool BitVector::LargeBVIsEmpty() const{
	assert(IsLargeBV());
	const LargeBV* largebv = ((LargeBV*)smallbv);
	return largebv->IsEmpty();
}


size_t BitVector::LargeBVFindIndexLargerThan(size_t index) const{
	const LargeBV* largebv = ((LargeBV*)smallbv);
	return largebv->FindIndexLargerThan(index);
}
*/
std::ostream& BitVector::operator<<(std::ostream& o){
	o << "BitVector dump not implemented";
	return o;
}
