# Configuração Local do Projeto (Setup)

Este guia descreve os passos necessários para configurar e rodar o projeto localmente.

## Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL em execução localmente (ou via Docker)
- Uma conta e projeto configurados no [Google Cloud Console](https://console.cloud.google.com/) para gerar credenciais OAuth2.

## 1. Configurando User Secrets
Para evitar salvar chaves sensíveis no controle de versão, utilizamos o `User Secrets` do .NET.
Navegue até a pasta do projeto da API:

```bash
cd src/MsTera.Api
```

E configure os seguintes segredos:

```bash
# Chave Secreta para gerar os JWTs (deve conter pelo menos 32 caracteres)
dotnet user-secrets set "Jwt:Key" "SUA_CHAVE_SECRETA_SUPER_SEGURA_AQUI_12345"

# Credenciais do Google OAuth2
dotnet user-secrets set "Authentication:Google:ClientId" "SEU_CLIENT_ID_DO_GOOGLE.apps.googleusercontent.com"
dotnet user-secrets set "Authentication:Google:ClientSecret" "SEU_CLIENT_SECRET_DO_GOOGLE"
```

## 2. Banco de Dados
Certifique-se de que a connection string para o seu PostgreSQL local esteja correta no `appsettings.Development.json` do `MsTera.Api` (ou defina nos User Secrets).

Rodar as migrações (na raiz do projeto ou na pasta do repositório):
```bash
dotnet ef database update --project src/Shared/MsTera.Shared.Infrastructure --startup-project src/MsTera.Api
```

## 3. Rodando o Projeto
Para iniciar a API:
```bash
dotnet run --project src/MsTera.Api
```
O Swagger estará disponível em `https://localhost:<porta>/swagger`.

## 4. Testando a Autenticação
- No Swagger, há um botão de `Authorize` onde você deve inserir o seu JWT (no formato `Bearer {token}`).
- Como a autenticação primária é feita via Google, você precisará acessar as rotas de login (como `/login-google`) via navegador para realizar a etapa de consentimento e obter o seu token JWT em resposta.
