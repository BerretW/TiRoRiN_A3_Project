timeout 
set dayzpath=""
cd /d %dayzpath%
@echo off
start "Arma2" /min  -port= -config=\config.cfg -cfg=\basic.cfg -profiles= -name= -cpuCount=2 -exThreads=1 -maxmem=2047 -noCB -mod=

timeout 

set becpath="@@bec@@"
cd /d %becpath%
start "bec1" /min "bec.exe" "--dsc"

exit

