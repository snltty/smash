@echo off
SET target=%~dp0

SET image=%1
if "%image%"=="" (SET image="snltty/smash.proxy")


for %%f in (smash.proxy) do (
	for %%p in (alpine) do (
		for %%r in (x64,arm64) do (
			dotnet publish ./%%f -c release -f net7.0 -o ./public/publish/docker/linux-%%p-%%r/%%f  -r %%p-%%r  --self-contained true -p:TieredPGO=true  -p:DebugType=none -p:DebugSymbols=false  -p:PublishSingleFile=true -p:PublishTrimmed=true -p:EnableCompressionInSingleFile=true -p:DebuggerSupport=false -p:EnableUnsafeBinaryFormatterSerialization=false -p:EnableUnsafeUTF7Encoding=false -p:HttpActivityPropagationSupport=false -p:InvariantGlobalization=true  -p:MetadataUpdaterSupport=false  -p:UseSystemResourceKeys=true  -p:TrimMode=partial
			move "public\\publish\\docker\\linux-%%p-%%r\\%%f\\%%f" "public\\publish\\docker\\linux-%%p-%%r\\%%f\\%%f.run"
			echo F|xcopy "smash.proxy\\Dockerfile-%%p" "public\\publish\\docker\\linux-%%p-%%r\\%%f\\Dockerfile-%%p"  /s /f /h /y
		)

		rem cd public/publish/docker/linux-%%p-x64/%%f
		rem docker buildx build -f "%target%\\public\\publish\\docker\\linux-%%p-x64\\%%f\\Dockerfile-%%p" --platform="linux/x86_64"  --force-rm -t "%image%-%%p-x64" . --push
		rem cd ../../../../

		rem cd public/publish/docker/linux-%%p-arm64/%%f
		rem docker buildx build -f "%target%\\public\\publish\\docker\\linux-%%p-arm64\\%%f\\Dockerfile-%%p" --platform="linux/arm64"  --force-rm -t "%image%-%%p-arm64" . --push
		rem cd ../../../../
	)
)