@echo off

del "C:\ProgramData\Unity\Unity_lic.ulf" /f /q

cd "C:\Program Files\Unity\Hub\Editor\2022.3.33f1\Editor"

Unity.exe -batchmode -username technician@lincoln.ac.uk -password Socks2021 -serial E4-3FYC-PKAQ-747G-93WB-UKVG -quit