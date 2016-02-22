
#include "VBPruning.h"
#include "IChessStrategy.h"
#include "ChessExecution.h"
#include <map>
#include "SyncVar.h"

bool VBPruning::ComputePremptionVars(ChessExecution* exec,IQueryEnabled *qEnabled)
{
	VBPruning::vars.clear();
	size_t length = exec->NumTransitions();
	for(size_t i = 1; i < length ; i++) {
		ChessTransition prev =  exec->Transition(i -1);
		ChessTransition curr = exec->Transition(i);

		if(prev.tid == curr.tid)
			continue;

		if(!qEnabled->IsEnabledAtStep(i,prev.tid))
			continue;
		// Context Swtich is pre-emptive one

		VBPruning::vars.insert(curr.var);
	}
	return vars.size() > var_bound;
}