# run netstat
netstat -ano | findstr "LISTENING"

# run pico
java -jar plantuml-1.2025.4.jar -picoweb

# run http.server
C:\Users\pavel\AppData\Local\Programs\Python\Python313\python.exe -m http.server 9000