SE HOUVER ALTERA��O NA PORTA DA APLICA��O, LEMBRAR DE ALTERAR NOS SEGUINTES ARQUIVOS:
Tools\BindCertificates.cmd
App.config


PARA GERAR UM NOVO CERTIFICADO
Executar o arquivo:
Tools\CreateCertificate.bat

Abrir o console do windows (windows+r > mmc)
Arquivo > Adicionar ou remover snap-in
Certificados > Adicionar
Conta de computador > Avancar > Concluir
Ok
Expandir Certificados (computador local)
Pessoal > Certificados
Encontre o certificado localhost adicionado com data de validade ainda n�o expirado (o que foi criado agora), abra Propriedades do certificado
	Nas propriedades v� na aba Detalhes e procure por Impress�o Digital (em ingl�s: thumbprint)
	Copie o valor e remova os espa�os
	V� no arquivo BindCertificate.bat e altere o valor do campo "certhash=" pela Impress�o digital copiada (sem espa�os)
	Salve
Fecha tudo e gere o novo instalador de acordo com o passo a passo abaixo (PUBLICA��O)



PUBLICA��O (Gerar instalador atualizado):
Alterar o modo para RELEASE
Executar o ProductCodeGenerator (Escolher op��o 1)
Compilar o projeto DocumentSignerInstaller
Abrir a pasta do projeto DocumentSignerInstaller, ir para pasta Release e copiar o arquivo DocumentSignerInstaller.msi