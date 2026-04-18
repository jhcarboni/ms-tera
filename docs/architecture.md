# Arquitetura do Sistema

## Padrão: Monolito Modular
O **Ms-Tera** foi desenhado como um Monolito Modular. Essa abordagem oferece a facilidade de deploy e desenvolvimento de um monolito tradicional, mas preserva a separação estrita de domínios (Bouded Contexts do DDD) típica de microsserviços. 

Caso o sistema cresça e haja necessidade de separar os serviços no futuro, o esforço de extração de um módulo para um microsserviço independente será substancialmente menor.

## Stack Tecnológica
- **C# / .NET 10:** Framework principal focado em alta performance e manutenibilidade (Web API).
- **PostgreSQL:** Banco de dados relacional escolhido por sua robustez e features avançadas.
- **Entity Framework Core (Npgsql):** ORM utilizado para abstrair o acesso a dados e gerenciar migrações.
- **OAuth2 / Google Auth & JWT:** Autenticação primária baseada nas contas do Google combinada com JWT para autorização das APIs.
- **OpenAPI/Swagger:** O projeto utiliza `Swashbuckle.AspNetCore` (v6.5.0). *Nota de Arquitetura:* Optou-se por não utilizar o pacote nativo `Microsoft.AspNetCore.OpenApi` (.NET 9+) simultaneamente para evitar conflitos de dependências transitivas com as bibliotecas-base do `Microsoft.OpenApi`.

## Regras e Convenções
1. **Isolamento de Módulos:** Um módulo (ex: `Users`) não deve acessar diretamente o banco de dados de outro módulo. Toda comunicação entre módulos deve acontecer via chamadas in-memory explícitas (ex: Interfaces públicas no pacote `Abstractions`) ou eventos de domínio (Domain Events).
2. **Clean Architecture Interna:** Dentro de cada módulo, o código deve ser separado nas camadas lógicas tradicionais:
   - *Domain:* Entidades de negócio, regras principais e interfaces.
   - *Application:* Casos de Uso (ex: Handlers do CQRS).
   - *Infrastructure:* Implementação de repositórios, DbContext do EF Core para o módulo e integrações externas.
   - *Endpoints/Presentation:* Exposição de rotas (Controllers ou Minimal APIs).
3. **Shared Kernel:** O pacote `Shared` contém lógicas transversais (cross-cutting concerns) aplicáveis a todos os módulos, como interfaces genéricas de repositório, mensageria interna, middlewares de tratamento de erro, etc.

## Testes Automatizados
- **Testes Unitários:** O foco é testar isoladamente o `Domain` e a camada `Application` usando xUnit e Moq.
- **Testes de Integração:** Validar o sistema de ponta a ponta configurando um banco de dados real via `Testcontainers`, garantindo que consultas, transações e roteamentos estejam funcionando juntos como esperado.
