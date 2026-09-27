# Life Played Unity Client

This project is the mobile presentation/runtime client for Life Played.

## Editor

- Unity 6000.3.25f1
- URP 17.3
- Input System 1.20.0
- Addressables 2.10.3
- Addressables for Android 1.1.0

The Unity client is **not authoritative** for XP, entitlements, currency, or real-world rewards.

Client layering:

`Presentation → Application → DomainBridge`

Infrastructure implements Application interfaces. Platform adapters remain isolated.

Do not commit Unity-generated `Library`, `Temp`, `Obj`, build, or UserSettings directories.
