@echo off

rd /s /q public\\publish
rd /s /q public\\publish-zip
mkdir public\\publish-zip

set DOTNET_TieredPGO=1

dotnet publish ./smash/smash -c release -f net7.0-windows -r win-x64 -o ./public/publish/win-x64  --self-contained true -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false  -p:PublishSingleFile=true  -p:EnableCompressionInSingleFile=true -p:DebuggerSupport=false  -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true
dotnet publish ./smash/smash -c release -f net7.0-windows -o ./public/publish/any -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false -p:DebuggerSupport=false  -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true

7z a -tzip ./public/publish-zip/smash-win-x64.zip ./public/publish/win-x64/*
7z a -tzip ./public/publish-zip/smash-any.zip ./public/publish/any/*


