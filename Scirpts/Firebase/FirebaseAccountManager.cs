using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class FirebaseAccountManager : MonoBehaviour
{
    private FirebaseAuth auth;
    
    private string email = "";
    private string password = "";
    private string nickname = "";
    
    private string statusMessage = "";

    private bool isInitialized = false;
    private bool isLoggedIn = false;
    private bool isSignUpMode = false;

    public GameObject login_Popup;
    public GameObject signUp_Popup;
    
    public TMP_InputField inputField_Id;
    public TMP_InputField inputField_Password;
    
    public Button loginButton;
    public Button loginSignUpButton;

    public TMP_InputField signupInputField_Id;
    public TMP_InputField signupInputField_Password;
    
    public Button signupButton;

    private bool OnloginPopup = false;
    
    public TextMeshProUGUI signupResult_Text;
    public TextMeshProUGUI loginResult_Text;
    
    public Button back_Button;
    private void Awake()
    {
        loginButton.onClick.AddListener(()=>SignIn(inputField_Id.text, inputField_Password.text));
        loginSignUpButton.onClick.AddListener(LoginSignupButton_Click);
        signupButton.onClick.AddListener(SignupButton_Click);
        back_Button.onClick.AddListener(BackButton_Click);
    }
    private void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => // 제대로 설치되어 있는지 확인하고, 문제 있으면 고치려 시도합니다. 초기화 전에 호출해야됨
        {
            if (task.Result == DependencyStatus.Available)
            {
                auth = FirebaseAuth.DefaultInstance; // 초기화
                isInitialized = true;
                statusMessage = "Firebase 초기화 완료";
            }
            else
            {
                statusMessage = $"Firebase 초기화 실패: {task.Result}";
            }
        });
    }
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (OnloginPopup == false)
            {
                OnloginPopup = true;
                login_Popup.SetActive(true);
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            var current = EventSystem.current.currentSelectedGameObject;
            
            if (current == inputField_Id.gameObject)
            {
                Focus(inputField_Password);
            }
            else if (current == inputField_Password.gameObject)
            {
                Focus(inputField_Id);
            }
            if (current == signupInputField_Id.gameObject)
            {
                Focus(signupInputField_Password);
            }
            else if (current == signupInputField_Password.gameObject)
            {
                Focus(signupInputField_Id);
            }
        }
    }
    void Focus(TMP_InputField field)
    {
        EventSystem.current.SetSelectedGameObject(field.gameObject, new BaseEventData(EventSystem.current));
        field.ActivateInputField();
    }
    private void LoginSignupButton_Click()
    {
        inputField_Id.text = "";
        inputField_Password.text = "";
        login_Popup.SetActive(false);
        signUp_Popup.SetActive(true);
        loginResult_Text.text = "";
        loginResult_Text.gameObject.SetActive(false);
    }

    private void SignupButton_Click()
    {
        CreateAccount(signupInputField_Id.text, signupInputField_Password.text);
    }

    private void BackButton_Click()
    {
        signupInputField_Id.text = "";
        signupInputField_Password.text = "";
        signUp_Popup.SetActive(false);
        login_Popup.SetActive(true);
        signupResult_Text.text = "";
        signupResult_Text.gameObject.SetActive(false);
    }
    private void CreateAccount(string email, string password)
    {
        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            signupResult_Text.gameObject.SetActive(true);
            if (task.IsCanceled || task.IsFaulted)
            {
                signupResult_Text.text = "Signup Failed";
                return;
            }
            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            signupResult_Text.text = "Signup Success";
            signupInputField_Id.text = "";
            signupInputField_Password.text = "";
            signUp_Popup.SetActive(false);
            login_Popup.SetActive(true);
            CreateUserDocument(newUser.UserId, email);
            signupResult_Text.text = "";
            signupResult_Text.gameObject.SetActive(false);
        });
    }
    private void CreateUserDocument(string uid, string email)
    {
        DocumentReference userDoc = Managers.FirestoreManager.firestore.Collection("users").Document(uid);
        var userData = new
        {
            email = email,
            createAt = Timestamp.GetCurrentTimestamp(),
            role = "user"
        };
        userDoc.SetAsync(userData).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompletedSuccessfully == true)
            {
                Debug.Log("FireStore 사용자 문서 생성완료");
            }
            else
            {
                Debug.Log("FireStore 문서 생성 실패");
            }
        });
    }
    private void SignIn(string email, string password)
    {
        Debug.Log("로그인 하는 중");
        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            loginResult_Text.gameObject.SetActive(true);
            if (task.IsCanceled || task.IsFaulted)
            {
                loginResult_Text.text = "Login Failed";
                Debug.LogError(task.Exception.Message);
                return;
            }
            isLoggedIn = true;
            loginResult_Text.text = "Login Success";
            SceneManager.LoadSceneAsync("Tycoon");
        });
    }
}