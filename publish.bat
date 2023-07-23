@echo off

rd /s /q public\\publish
rd /s /q public\\publish-zip
mkdir public\\publish-zip

set DOTNET_TieredPGO=1

dotnet publish ./smash -c release -f net7.0-windows -r win-x64 -o ./public/publish/smash-win-x64-single  --self-contained true -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false  -p:PublishSingleFile=true  -p:EnableCompressionInSingleFile=true -p:DebuggerSupport=false  -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true
dotnet publish ./smash -c release -f net7.0-windows -o ./public/publish/smash-any -p:PublishSingleFile=true --self-contained false -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false -p:DebuggerSupport=false  -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true
7z a -tzip ./public/publish-zip/smash-any.zip ./public/publish/smash-any/*
7z a -tzip ./public/publish-zip/smash-win-x64-single.zip ./public/publish/smash-win-x64-single/*


for %%f in (smash.proxy) do (
	for %%r in (win-x64,win-arm64,linux-x64,linux-arm64,osx-x64,osx-arm64) do (
		dotnet publish ./%%f -c release -f net7.0 -o ./public/publish/%%f-%%r-single  -r %%r  --self-contained true -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false  -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -p:DebuggerSupport=false -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true  -p:TrimMode=partial
		
		7z a -tzip ./public/publish-zip/%%f-%%r-single.zip ./public/publish/%%f-%%r-single/*

		dotnet publish ./%%f -c release -f net7.0 -r %%r -o ./public/publish/%%f-any/%%r  -p:PublishSingleFile=true --self-contained false
	)
	7z a -tzip ./public/publish-zip/%%f-any.zip ./public/publish/%%f-any/*
)
