netsh http delete urlacl url=https://+:8070/
netsh http delete sslcert ipport=0.0.0.0:8070

netsh http add urlacl url=https://+:8070/ sddl=D:(A;;GA;;;WD)
netsh http add sslcert ipport=0.0.0.0:8070 certhash=e836c47ffeefa53d5f132f7db1fe595f9dd7cbe4 appid={D94F01E7-0826-4D8F-9688-EF2B7EB13BF4}
exit 0