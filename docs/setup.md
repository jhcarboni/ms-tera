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

## 4. Testando a Autenticação e o OAuth2

**Passo 1: Verifique as Permissões no Google Cloud**
Antes de testar o login, acesse o painel do Google Cloud (onde o ClientId foi criado) e garanta que você adicionou a seguinte URL na aba "URIs de redirecionamento autorizados":
👉 `http://localhost:<porta>/signin-google` *(verifique no log a porta em que a API subiu, ex: 5282)*.

**Passo 2: Faça o Login via Navegador**
No navegador, acesse manualmente a rota de desafio:
👉 `http://localhost:<porta>/api/auth/login-google`
- Você será redirecionado para a tela de login do Google.
- Após escolher sua conta e aprovar as permissões, o Google redirecionará você de volta para a API.
- Se tudo der certo, você verá uma tela JSON com a mensagem de sucesso e o seu **Token JWT**. Copie **APENAS** o grande texto gerado no campo `"token"`.

**Passo 3: Insira o Token no Swagger**
- Acesse o Swagger da aplicação (`http://localhost:<porta>/swagger`).
- Clique no botão **Authorize** (no topo).
- Cole o Token copiado no campo de texto. (Não é preciso digitar a palavra "Bearer", nosso Swagger está configurado para injetá-la automaticamente).
- Clique em *Authorize* e feche o modal.

**Passo 4: Valide nas Rotas Protegidas**
- Expanda o endpoint protegido, por exemplo o **`GET /api/auth/me`**.
- Clique em *Try it out* e *Execute*.
- Se o token estiver correto, a API validará o acesso e retornará 200 OK com os seus dados lidos diretamente do JWT (Email, Nome e ID Google).
