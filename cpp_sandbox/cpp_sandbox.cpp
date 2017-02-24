#include "stdafx.h"

template< class T, class I = int > class CUtlMemory {
public:
	enum { EXTERNAL_BUFFER_MARKER = -1, EXTERNAL_CONST_BUFFER_MARKER = -2, };

	T* m_pMemory;
	int m_nAllocationCount;
	int m_nGrowSize;
};

template< class T, class A = CUtlMemory<T> > class CUtlVector {
	typedef A CAllocator;
public:
	CAllocator m_Memory;
	int m_Size;
};

EXPORT INT32 cpp_test() {
	CUtlVector<int>* V = new CUtlVector<int>();

	int Base = (int)V;
	int m_Memory = (int)&V->m_Memory;
	int Diff = m_Memory - Base;

	int m_pMemory = (int)&V->m_Memory.m_pMemory;
	int Diff2 = m_pMemory - Base;

	return 0;
}