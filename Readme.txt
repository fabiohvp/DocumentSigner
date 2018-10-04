SE HOUVER ALTERAÇÃO NA PORTA DA APLICAÇÃO, LEMBRAR DE ALTERAR NOS SEGUINTES ARQUIVOS:
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
Encontre o certificado localhost adicionado com data de validade ainda não expirado (o que foi criado agora), abra Propriedades do certificado
	Nas propriedades vá na aba Detalhes e procure por Impressão Digital (em inglês: thumbprint)
	Copie o valor e remova os espaços
	Vá no arquivo BindCertificate.bat e altere o valor do campo "certhash=" pela Impressão digital copiada (sem espaços)
	Salve
Fecha tudo e gere o novo instalador de acordo com o passo a passo abaixo (PUBLICAÇÃO)



PUBLICAÇÃO (Gerar instalador atualizado):
Alterar o modo para RELEASE
Executar o ProductCodeGenerator (Escolher opção 1)
Compilar o projeto DocumentSignerInstaller
Abrir a pasta do projeto DocumentSignerInstaller, ir para pasta Release e copiar o arquivo DocumentSignerInstaller.msi