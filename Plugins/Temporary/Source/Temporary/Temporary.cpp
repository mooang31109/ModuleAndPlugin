#include "Temporary.h"
#include "Modules/ModuleManager.h"


void FTemporaryModule::StartupModule()
{
	UE_LOG(LogTemp, Warning, TEXT("Temporary plugin module started."));
}

void FTemporaryModule::ShutdownModule()
{
	UE_LOG(LogTemp, Warning, TEXT("Temporary plugin module shut down."));
}

IMPLEMENT_MODULE(FTemporaryModule, Temporary);