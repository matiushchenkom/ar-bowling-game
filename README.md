# AR Bowling🎳
This is a mobile AR game I developed to explore the intersection of physical world interactions and Unity’s physics engine. Built with **Unity 6** and **AR Foundation**, the goal was to create a responsive bowling experience that feels natural on a mobile screen.

## 🛠 What's under the hood?
* **Custom Mechanics:** Instead of relying on heavy interaction frameworks, I decided to build the core mechanics from scratch to keep the project lightweight and have full control over the physics.
* **Custom Gesture Physics:** I spent quite a bit of time fine-tuning the swipe-to-throw logic. It’s not just a simple trigger; the script captures the velocity of your finger swipe and translates that exact momentum into a 3D force vector using `Rigidbody.AddForce`.
* **Spatial Consistency:** To stop the virtual lane from "sliding" around your floor, I implemented **AR Anchors**. This keeps the game world locked in place even if you move your phone around quickly.
* **Plane Detection:** The game scans the environment and identifies horizontal surfaces to place the lane. It’s optimized to handle different lighting conditions and floor textures.
* **Cinematic Intro:** I added a custom `PreviewManager` using C# Coroutines to handle smooth UI transitions and project info before the game starts. It’s a small detail, but it makes the app feel polished.

## 📺 See it in action
You can check out the gameplay recorded in XR Simulation here:  
**https://drive.google.com/file/d/1X6f9GRcWN3Dxs7IHvi1wP7wrzOYDS8en/view?usp=drive_link**

## 💻 Tech Stack
* **Engine:** Unity 6 (6000.3.9f1)
* **Framework:** AR Foundation (ARCore/ARKit)
* **Scripting:** C# 
* **Assets:** Low-poly models made/optimized in **Blender**

## ⚙️ How to get it running
1. Clone the repo.
2. Open it in Unity 6.
3. Make sure you have **ARCore** or **ARKit** enabled in XR Plug-in Management.
4. Build and deploy to your phone.

---
**Marharyta Matiushchenko**
[LinkedIn](https://www.linkedin.com/in/marharyta-matiushchenko/) | [ArtStation](https://www.artstation.com/marharyta-matiushchenko)
