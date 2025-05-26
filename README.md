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

1. **Download** the latest release from the [Releases page](https://github.com/Bill3621/CustomItems/releases).
2. Or **clone** the repository if you want to build from source.
3. If building from source:
   - **Open** `CustomItems-LabAPI.sln` in Visual Studio.
   - **Restore** NuGet packages if required.
   - **Build** the solution.
   - **Place** the built file into the plugins folder of your server.

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
