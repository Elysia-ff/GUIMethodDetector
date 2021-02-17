# GUIMethodDetector
Detects the methods and animation events that are linked on inspector

![image](https://user-images.githubusercontent.com/45890606/108181983-e4338680-714b-11eb-9721-802b0b9d0024.png)

1. If you want to detect the methods on specific object, assign it, hit the button.

2. Detector will find all prefabs under Assets folder.
 
3. Same behavior as 2 but under the current Scene.

4. This dark black line locates the object that has inspector methods  
Click it to ping object.  
And the indented lines below are the method names.

5. Events linked on Animation Clip will be displayed like this.


# How it works
This searches all the public / private but serializable fields and properties using reflection on the objects.  
The type of them must inherit UnityEventBase.

But the fields/properties containing UnityEventBase cannot be found.  
(see https://github.com/Elysia-ff/GUIMethodDetector/blob/d153ea6a2a6a75b07d875b71e6c11a3947ed7d99/Assets/GUIMethodDetector/Sample/SamplePrefab.cs#L27)

If you have some classes like this, Add your own sub detector.  
(see https://github.com/Elysia-ff/GUIMethodDetector/blob/main/Assets/GUIMethodDetector/Editor/SubDetector/EventTriggerSubDetector.cs)


# In my opinion
Do not use inspector method. (also string method like StartCoroutine("DoSomething");)  
It's easy to use but makes us harder to debug because 'Find All References' does not work with this.  
And if we obfuscate our codes, they must be skipped when obfuscating due to the way they behave.
