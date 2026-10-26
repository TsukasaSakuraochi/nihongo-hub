# NihongoHub

日语学习平台。目前正在开发的第一个功能是**单词本与单词检验**：用户登录后可以创建单词本，录入单词（汉字、假名、音调、词义），再用它们做检验。

*English: NihongoHub is a Japanese-learning platform. The first feature under development is a vocabulary notebook with quizzes.*

> [!WARNING]
> **本项目仍处于早期开发阶段，目前无法使用。**
> 没有发布任何可运行的版本；接口、数据结构、配置文件格式随时可能发生不兼容的改动。请不要用于生产环境，也不要依赖它保存重要数据。
>
> *This project is in early development and is not usable yet. There is no runnable release, and everything may change without notice.*

## 计划中的功能

以下均为计划，尚未完成：

- 用户注册与登录，区分管理员与普通用户
- 单词本：创建单词本并用标签分类
- 单词：汉字（可空）、假名、音调（アクセント）、词义、备注
- 单词检验（由前端组织，可跨单词本合并）
- 管理员对用户的管理
- 同一套 HTTP API 同时服务网页与以后的手机 App

## 技术栈

- 后端：C#、ASP.NET Core（net10.0，Minimal API）
- 数据库：[LiteDB](https://www.litedb.org/)（单文件）
- 认证：JWT
- 日志：Serilog
- 前端：HTML / CSS / JavaScript（静态文件，由后端托管）

## 仓库结构

```
src/    C# 后端源码
web/    前端源码（HTML / CSS / JS）
```

## 开发状态

- [x] 项目骨架
- [ ] 配置与日志
- [ ] 数据库与数据模型
- [ ] 认证
- [ ] 单词本 API
- [ ] 前端页面

## 开发环境（仅供开发者）

需要 [.NET 10 SDK](https://dotnet.microsoft.com/download)。

```bash
dotnet build src
```

目前不提供使用说明，也不接受使用上的咨询。

## 许可证

本项目以 [MIT 许可证](LICENSE) 发布。
