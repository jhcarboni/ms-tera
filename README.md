# Ms-Tera: Sistema de Agendamento para Clínicas de Terapias

Este repositório contém o código-fonte do backend para um sistema de agendamento focado em clínicas de terapias.

## 📖 Contexto do Projeto
O objetivo deste projeto é fornecer uma plataforma robusta e escalável para gerenciar agendamentos, pacientes e profissionais. Todo o backend foi desenhado focando em manutenibilidade e separação clara de responsabilidades.

## 🏗️ Arquitetura
O projeto segue o padrão **Monolito Modular**, estruturado utilizando as melhores práticas do **Clean Code** e dos princípios **SOLID**.

Para mais detalhes arquiteturais e stack tecnológica, consulte a documentação detalhada:
- [Documentação de Arquitetura](./docs/architecture.md)
- [Guia de Setup e Execução](./docs/setup.md)

## 🚀 Tecnologias Principais
- **Linguagem:** C# 14 (.NET 10 LTS)
- **Framework:** ASP.NET Core Web API
- **Banco de Dados:** PostgreSQL
- **ORM:** Entity Framework Core (via Npgsql)
- **Autenticação:** Google OAuth2 (OpenID Connect) & JWT
- **Testes:** xUnit, Moq e Testcontainers

## 📂 Estrutura da Solution
```text
src/
  ├── MsTera.Api/                  (Host, Web API, Configurações de DI)
  ├── Modules/
  │   └── MsTera.Modules.Users/    (Módulo responsável pela autenticação e gestão de usuários)
  └── Shared/
      ├── MsTera.Shared.Abstractions/ (Interfaces globais, eventos de domínio, etc)
      └── MsTera.Shared.Infrastructure/ (Configuração de DB, Auth base, middlewares globais)
tests/
  (Testes Unitários e de Integração)
```
