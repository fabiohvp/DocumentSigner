C:\OpenSSL-Win64\bin\openssl.exe req -x509 -nodes -days 1024 -newkey rsa:2048 -keyout localhost.pem -out localhost.pem -config server.csr.cnf
C:\OpenSSL-Win64\bin\openssl.exe pkcs12 -export -out localhost.pfx -in localhost.pem -name "localhost" -passout pass:123456
exit 0