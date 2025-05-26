# 🚀 CustomItems LabAPI

CustomItems LabAPI is a plugin framework for creating and managing custom items in **SCP: Secret Laboratory**.  
It provides a structured API for defining, registering, and handling custom items, along with example implementations and core utilities for event handling and command management.

---

## ✨ Features

- 🛠️ Define custom items with unique behaviors
- 📦 Register and manage custom items via API
- 💣 Example custom items (e.g., EMP Grenade)
- ⚡ Event handling for custom item actions
- 💬 Command system for interacting with custom items
- 📝 Logging utilities for debugging and monitoring
- 🧪 **Item spawning support will be added in the future!**

---

## 🏁 Getting Started

1. **Clone** the repository or download the source code.
2. **Open** `CustomItems-LabAPI.sln` in Visual Studio.
3. **Restore** NuGet packages if required.
4. **Build** the solution.
5. **Place** the built file into the plugins folder of your server.

---

## 🛡️ Usage

- To create a new custom item, **inherit** from `API/CustomItem.cs` and implement required methods.
- **Register** your custom item in `API/CustomItems.cs`.
- Use the **command system** to interact with custom items in-game or via console.

---

## 📚 Example

See [`API/Example/EMPGrenade.cs`](CustomItems-LabAPI/API/Example/EMPGrenade.cs) for an example of how to implement a custom item.

---

## 🤝 Contributing

Contributions are welcome!  
Please **fork** the repository and submit a pull request with your changes.

---

## 📄 License

This project is licensed under the **MIT License**.
