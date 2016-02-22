/********************************************************
*                                                       *
*     Copyright (C) Microsoft. All rights reserved.     *
*                                                       *
********************************************************/

#include "SSEstimator.h"
//#include <windows.h> 
typedef unsigned __int64 UINT64;

UINT64 choose(size_t n, size_t k) {
	if (k > n)
		return 0;

	if (k > n/2)
		k = n-k; // Take advantage of symmetry

	long double accum = 1;
	for (size_t i = 1; i <= k; i++)
		accum = accum * (n-k+i) / i;

	return (UINT64)(accum + 0.5); // avoid rounding error
}

UINT64 factorial(size_t n){
	if(n > 40)
		return 0; // dont even bother
	UINT64 ret = 1;
	for(size_t i=2; i<n; i++){
		ret *= i;
	}
	return ret;
}

void SSEstimator::OnExecutionBegin(const IChessExecution* exec){
	size_t sum = 0;
	if(exec->NumTransitions() == 0){
		stackSplit.clear();
		return;
	}
	for(size_t i=0; i<stackSplit.size(); i++){
		assert(sum < exec->NumTransitions());
		if(sum + stackSplit[i] >= exec->NumTransitions()){
			stackSplit[i] = exec->NumTransitions()-sum;
			stackSplit.resize(i+1);
			return;
		}
		sum = sum + stackSplit[i];
	}

	size_t execSize = exec->NumTransitions();
	if(execSize > exec->GetInitStack()){
		// A ExecutionBegin with execSize, means that we backtracked at execSize-1
		if(execSize > numBacktracksAtLevel.size()){
			numBacktracksAtLevel.resize(execSize);
		}	
		numBacktracksAtLevel[execSize-1]++; 
	}
}
void SSEstimator::OnExecutionEnd(const IChessExecution* exec, int numExecs){
	size_t sum = 0;

	for(size_t n=0; n<stackSplit.size(); n++){
		sum += stackSplit[n];
	}

	size_t diff = exec->NumTransitions() - sum;
	if(diff > 0){
		stackSplit.push_back(diff);
	}

	size_t i;
	sum = 0;
	for(i=0; i<exec->NumTransitions()-exec->GetInitStack() && i<prevExec.size(); i++){
		if(exec->Transition(i+exec->GetInitStack()).tid != prevExec[i])
			break;
	}
	// exec and prevExec differ at transition i - so backtrack is happening at state i
	for(size_t k=i; k<prevExec.size(); k++){
		size_t ss = numExecs - whenPushed[k];
		// subtree at k had ss execs
		if(stats.size() <= k){
			stats.resize(k+1, Info());
		}
		stats[k].sum += ss;
		stats[k].num ++;
		if(stats[k].max < ss) stats[k].max = ss;
		if(!stats[k].min || stats[k].min > ss) stats[k].min = ss;
	}
	prevExec.resize(i);
	whenPushed.resize(i);
	for(; i<exec->NumTransitions()-exec->GetInitStack(); i++){
		prevExec.push_back(exec->Transition(i+exec->GetInitStack()).tid);
		whenPushed.push_back(numExecs);
	}

}

void SSEstimator::DisplayEstimate(std::ostream& o, const IChessExecution* exec, int numExecs){
	int numBacktracks = 1;

	std::vector<size_t> estimates;

	size_t minBacktracks = 1;
	size_t firstBacktrackLevel = 0;
	size_t earliestSeenBacktrack = 0;

	double sum_fanout=0;
	size_t num_fanout=0;
	for(size_t i=1; i<stats.size(); i++){
		if(stats[i].num > 1 && stats[i-1].num > 1 && stats[i].sum/stats[i].num > 10){
			//fanout_{i-1} = stats[i-1].sum/stats[i-1].num /(stats[i].sum/stats[i].num)
			sum_fanout += (stats[i-1].sum* stats[i].num * 1.0/ (stats[i-1].num* stats[i].sum));
			num_fanout++;
		}
	}
	double avg_fanout = 0;
	if(num_fanout){
		avg_fanout = sum_fanout/num_fanout;
	}

	int totEnabled = 0;
	int maxEnabled = 0;
	int K = 0;
	int C = Chess::GetOptions().preemption_bound;
	int B = 0;

	for(size_t k = exec->GetInitStack(); k<exec->NumTransitions(); k++){
		size_t i = k-exec->GetInitStack();
		int numEnabled = exec->GetQueryEnabled()->NumEnabledAtStep(k);
		if(numEnabled == 0){
			break;
		}

		if(numEnabled > 1
			&& exec->Transition(k).op != SVOP::TASK_RESUME
			&& exec->Transition(k).op != SVOP::TASK_FORK
			&& exec->Transition(k).op != SVOP::TASK_END
			&& exec->Transition(k).op != SVOP::TASK_RESUME)
		{
			minBacktracks *= numEnabled;
			if(!firstBacktrackLevel)
				firstBacktrackLevel = i;
			K++;
			totEnabled += numEnabled;
			if(maxEnabled < numEnabled)
				maxEnabled = numEnabled;
			if(k != 0 && !exec->GetQueryEnabled()->IsEnabledAtStep(k, exec->Transition(k-1).tid))
				B++;
		}

		if(!earliestSeenBacktrack && i < stats.size() && stats[i].num > 0)
			earliestSeenBacktrack = i;

		if(i < stats.size() && stats[i].num > 0){
			// now we have a good estimate of ssbelow - its the average state space seen at level i
			// we need a good estimate of ssabove, the number of times we will hit level i
			double ssbelow = stats[i].sum/stats[i].num;

			// first estimate: ssabove = minBacktracks; this is an upperbound
			// This is pretty good when minBacktracks is not a big number (i.e. at the end of the search)
			UINT64 ssabove1 = minBacktracks;
			UINT64 ssest1 = stats[i].sum*ssabove1/stats[i].num;


			// second estimate : use the csb formula
			int N = totEnabled/K; // average enabled, approx reflects number of "threads"
			UINT64 ssabove2 = choose(K, C) * factorial(N+C+B);
			UINT64 ssest2 = stats[i].sum*ssabove2/stats[i].num;
			if(ssest2 < numExecs) ssest2 = 0; // no sense

			// use avg fanout
			UINT64 ssest3 = 0;
			if(avg_fanout){
				// find expected ssbelow at firstbacktracklevel, given ssbelow at level i
				long double ssbelowAtL= ssbelow;
				for(size_t l=i-1; l>=firstBacktrackLevel-1; l--){
					ssbelowAtL *= avg_fanout;
				}
				ssest3 = UINT64(ssbelowAtL+0.5);
			}
			if(ssest3 < numExecs){
				ssest3 = 0;
			}
			UINT64 ss = ssest1;
			if(!ss || ssest2 && ssest2 < ss) ss = ssest2;
			if(!ss || ssest3 && ssest3 < ss) ss = ssest3;

//			*GetChessErrorStream()<< "SS="<< ss << ": " << firstBacktrackLevel << "/" << earliestSeenBacktrack << " ";
			//<< ssest1 << " " << ssest2 << " " << ssest3 << " ";
			o << "~Est: " << ss << ' ';
			long double percentDone = ss ? numExecs*100.0/ss : 0;
			o << "%Done: " << (int)(percentDone+0.5) << ' ';
			return;
//			return (size_t)(percentDone+0.5);
		}
	}

	//	*GetChessErrorStream()<< "ToGo" << firstTwoEn << "," << minBack << " ";
	//
	//	if(estimates.size() > 0){
	//		size_t est =0;
	////		*GetChessErrorStream()<< "(";
	//		for(size_t j=0; j<estimates.size(); j++){
	////			*GetChessErrorStream()<< estimates[j] << ",";
	//			est += estimates[j];
	//		}
	////		*GetChessErrorStream()<< ")";
	//		return est/estimates.size();
	//	}
	//	return 0;

	//// basic idea, but didnt work
	////  N is the size of the execution
	////  At level k, SS(k) represents the state space below k
	////     En(k) is the number of threads enabled at k
	////  if tid(k-1) was disabled at k
	////      SS(k) = En(k).SS(k+1) /* every enabled thread sees a SS of size SS(k+1) */
	////  else
	////     if tid(k) == tid(k-1) /* All threads except the current will see a SS of SS(k+1)/(N-k) */
	////        SS(k) = SS(k+1) + (En(k)-1).SS(k+1)/(N-k) 
	////     else /* tid(k-1) will see SS(k+1)*(N-k), rest will see SS(k+1) */

	//size_t N = exec->NumTransitions();
	//if(N == 0)
	//	return 1;

	//size_t history_level = 0;
	//for(size_t i=0; i<stats.size(); i++){
	//	if(stats[i].num > 2){
	//		history_level = i;
	//		break;
	//	}
	//}

	//size_t k;
	//size_t SS;
	//if(history_level == 0 || history_level > N){
	//	k = N;
	//	SS = 1;
	//}
	//else{
	//	k = history_level;
	//	int numEnabled = 0;
	//	size_t first;
	//	exec->GetQueryEnabled()->NextEnabledAtStep(k, 0, first);
	//	size_t next = first;
	//	do{
	//		numEnabled++;
	//		exec->GetQueryEnabled()->NextEnabledAtStep(k, next, next);
	//	}while(next != first);

	//	size_t i = k;
	//	std::cout << "Stats " << i << "  " << stats[i].min << " " << stats[i].max << " " << stats[i].sum*1.0/stats[i].num << ' ' << stats[i].num << ' ' <<  stats[i].sum*1.0/stats[i].num * (stats[i].num+1)<< "\n";
	//	SS = stats[k].sum*numEnabled/stats[k].num;
	//}
	//do{
	//	//SS == SS(k)
	//	// compute SS(k-1) and store it in SS
	//	k--;
	//	int numEnabled = 0;
	//	size_t first;
	//	exec->GetQueryEnabled()->NextEnabledAtStep(k, 0, first);
	//	size_t next = first;
	//	do{
	//		numEnabled++;
	//		exec->GetQueryEnabled()->NextEnabledAtStep(k, next, next);
	//	}while(next != first);

	//	if(k == 0 || !exec->GetQueryEnabled()->IsEnabledAtStep(k, exec->Transition(k-1).tid)){
	//		SS = numEnabled*SS;
	//	}
	//	else{
	//		if(exec->Transition(k-1).tid == exec->Transition(k).tid){
	//			SS = SS + (numEnabled-1)*SS/(N-k);
	//		}
	//		else{
	//			SS = SS + SS*(N-k) + (numEnabled-2)*SS;
	//		}
	//	}
	//}while(k != 0);
	//return SS;
}
