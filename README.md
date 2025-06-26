<p align="center">
  <img src="https://img.shields.io/github/stars/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/forks/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/v/release/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/contributors/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/issues/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/issues-pr/Bill3621/CustomItems?style=for-the-badge" />
  <img src="https://img.shields.io/github/license/Bill3621/CustomItems?style=for-the-badge" />
</p>

# 🚀 CustomItems LabAPI

**CustomItems LabAPI** is a flexible and powerful plugin framework for creating and managing custom items in [![SCP: Secret Laboratory](https://img.shields.io/badge/SCP--SL-Game-7B00FF?logo=steam&logoColor=white&style=flat-square)](https://scpslgame.com/). It provides a structured API for defining, registering, and handling custom items with ease. This repo includes example implementations and core utilities to enhance plugin development with item behaviors, event hooks, and command interfaces.

---

## 📚 Table of Contents

- [✨ Features](#-features)
- [🏁 Getting Started](#-getting-started)
- [🛡️ Usage](#️-usage)
- [📦 Example](#-example)
- [🤝 Contributing](#-contributing)
- [📬 Contact](#-contact)
- [📄 License](#-license)
- [🔝 Back to Top](#-table-of-contents)

---

## ✨ Features

- 🛠️ Define custom items with unique behaviors
- 📦 Register and manage items through the API
- 💣 Includes example items like an **EMP Grenade**
- ⚡ Hook into custom item-related events
- 💬 Interact via a built-in command system
- 📝 Logging utilities for debugging and monitoring
- 🧪 *Future support for item spawning in the pipeline!*

---


## 🏁 Getting Started

1. **Download** the latest release from the [Releases Page](https://github.com/Bill3621/CustomItems/releases).
2. Or **clone** the repository to build from source:
   ```bash
   git clone https://github.com/Bill3621/CustomItems.git
   ````

3. **Build from source**:

   * Open `CustomItems-LabAPI.sln` in Visual Studio.
   * Restore any missing NuGet packages.
   * Build the solution.
   * Place the built DLL into your server's plugins folder.

---

## 🛡️ Usage

* Create a new item by inheriting from `API/CustomItem.cs`.
* Implement the required behavior logic.
* Register your item via `API/CustomItems.cs`.
* Use in-game or console commands to interact with your items.

> 💡 This modular setup allows easy integration and expansion!

---

## 📦 Example

Want a concrete implementation?
Check out [`API/Example`](CustomItems-LabAPI/API/Example) to see how to build a functioning custom item with event hooks and effects!

---

## 🤝 Contributing

We love community contributions! ❤️
To contribute:

1. Fork this repo
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📬 Contact

Created and maintained by **[Bill3621](https://github.com/Bill3621)**.
Feel free to open an issue or reach out via GitHub!

---

## 📄 License

This project is licensed under the [![MIT License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE).

---

## 🔝 [Back to Top](#-table-of-contents)
