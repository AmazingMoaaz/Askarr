/*!

=========================================================
* Argon Dashboard React - v1.0.0
=========================================================

* Product Page: https://www.creative-tim.com/product/argon-dashboard-react
* Copyright 2019 Creative Tim (https://www.creative-tim.com)
* Licensed under MIT (https://github.com/creativetimofficial/argon-dashboard-react/blob/master/LICENSE.md)

* Coded by Creative Tim

=========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

*/
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Alert } from "reactstrap";
import { useDispatch } from 'react-redux';
import { login } from "../store/actions/UserActions"

// reactstrap components
import {
  Card,
  CardBody,
  FormGroup,
  Form,
  Input,
  InputGroupAddon,
  InputGroupText,
  InputGroup,
  Col
} from "reactstrap";


function Login() {
  const [isLoading, setIsLoading] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [username, setUsername] = useState("");
  const [usernameChanged, setUsernameChanged] = useState(false);
  const [usernameInvalid, setUsernameInvalid] = useState(false);
  const [password, setPassword] = useState("");
  const [passwordInvalid, setPasswordInvalid] = useState(false);
  const [passwordChanged, setPasswordChanged] = useState(false);
  const [loginAttempted, setLoginAttempted] = useState(false);
  const [loginSuccess, setLoginSuccess] = useState(false);
  const [loginError, setLoginError] = useState("");

  const history = useNavigate();
  const dispatch = useDispatch();


  useEffect(() => {
    if (usernameChanged)
      triggerUsernameValidation();
  }, [usernameChanged]);


  useEffect(() => {
    if (passwordChanged)
      triggerPasswordValidation();
  }, [passwordChanged]);


  const validateUsername = () => {
    return /\S/.test(username);
  };


  const validatePassword = () => {
    return /\S/.test(password);
  };



  const triggerUsernameValidation = () => {
    setUsernameInvalid(!validateUsername());
  };

  const onUsernameChange = event => {
    setUsername(event.target.value);
    setUsernameChanged(true);
  };


  const onPasswordChange = event => {
    setPassword(event.target.value);
    setPasswordChanged(true);
  };


  const triggerPasswordValidation = () => {
    setPasswordInvalid(!validatePassword());
  };


  const onRememberMeChange = event => {
    setRememberMe(!rememberMe)
  };


  const onLogin = (e) => {
    e.preventDefault();

    triggerUsernameValidation();
    triggerPasswordValidation();

    if (validateUsername()
      && validatePassword()) {
      setIsLoading(true);

      dispatch(
        login({
          username: username,
          password: password,
          rememberMe: rememberMe
        }))
        .then(data => {
          if (data.ok) {
            history('/admin/');
          } else {
            setIsLoading(false);
            let error = "An unknown error occurred while logging in.";

            if (typeof (data.error) === "string")
              error = data.error;

            setLoginAttempted(true);
            setLoginError(error);
            setLoginSuccess(false);
          }
        });
    }
  };





  return (
    <>
      <Col lg="5" md="7">
        <Card 
          className="modern-card shadow-lg border-0"
          style={{
            background: 'linear-gradient(135deg, rgba(255, 255, 255, 0.95), rgba(255, 255, 255, 0.85))',
            backdropFilter: 'blur(20px)',
            border: '2px solid rgba(255, 255, 255, 0.3)',
            borderRadius: '1.5rem',
            boxShadow: '0 25px 60px rgba(0, 0, 0, 0.2)',
          }}
        >
          <CardBody className="px-lg-5 py-lg-5">
            <div className="text-center mb-5">
              <div 
                className="icon-shape mx-auto mb-4"
                style={{
                  width: '80px',
                  height: '80px',
                  borderRadius: '1.25rem',
                  background: 'linear-gradient(135deg, #4FD1C5 0%, #63B3ED 100%)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  boxShadow: '0 15px 40px rgba(79, 209, 197, 0.4)',
                  animation: 'pulse 2s ease-in-out infinite',
                }}
              >
                <i className="fas fa-sign-in-alt" style={{ fontSize: '2rem', color: '#fff' }}></i>
              </div>
              <h2 
                className="mb-2"
                style={{
                  fontWeight: '800',
                  background: 'linear-gradient(135deg, #4FD1C5 0%, #63B3ED 100%)',
                  WebkitBackgroundClip: 'text',
                  WebkitTextFillColor: 'transparent',
                  backgroundClip: 'text',
                }}
              >
                Welcome Back
              </h2>
              <p className="text-muted" style={{ fontSize: '0.95rem' }}>
                Sign in to continue to Askarr
              </p>
            </div>
            <Form onSubmit={onLogin} role="form">
              <FormGroup className={usernameInvalid || (usernameChanged && username === "") ? "has-danger" : usernameChanged ? "has-success" : ""}>
                <InputGroup 
                  className="mb-3"
                  style={{
                    background: 'rgba(255, 255, 255, 0.8)',
                    borderRadius: '0.75rem',
                    border: usernameInvalid ? '2px solid #FC8181' : usernameChanged && username !== "" ? '2px solid #4FD1C5' : '2px solid rgba(79, 209, 197, 0.3)',
                    boxShadow: '0 5px 15px rgba(0, 0, 0, 0.08)',
                    transition: 'all 0.3s ease',
                  }}
                >
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText style={{ background: 'transparent', border: 'none', paddingLeft: '1rem' }}>
                      <i className="fas fa-user" style={{ color: '#4FD1C5' }} />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Input 
                    placeholder="Username" 
                    value={username} 
                    onChange={onUsernameChange} 
                    type="text"
                    style={{ 
                      background: 'transparent', 
                      border: 'none',
                      fontSize: '0.95rem',
                      fontWeight: '500',
                    }}
                  />
                </InputGroup>
                {
                  usernameInvalid ? (
                    <Alert 
                      color="danger"
                      style={{
                        background: 'linear-gradient(135deg, rgba(252, 129, 129, 0.15), rgba(245, 101, 101, 0.05))',
                        border: '1px solid rgba(252, 129, 129, 0.3)',
                        borderRadius: '0.75rem',
                        color: '#FC8181',
                        fontWeight: '600',
                      }}
                    >
                      <i className="fas fa-exclamation-circle mr-2"></i>
                      Username is required
                    </Alert>)
                    : null
                }
              </FormGroup>
              <FormGroup className={passwordInvalid || (passwordChanged && password === "") ? "has-danger" : passwordChanged ? "has-success" : ""}>
                <InputGroup 
                  className="mb-3"
                  style={{
                    background: 'rgba(255, 255, 255, 0.8)',
                    borderRadius: '0.75rem',
                    border: passwordInvalid ? '2px solid #FC8181' : passwordChanged && password !== "" ? '2px solid #4FD1C5' : '2px solid rgba(79, 209, 197, 0.3)',
                    boxShadow: '0 5px 15px rgba(0, 0, 0, 0.08)',
                    transition: 'all 0.3s ease',
                  }}
                >
                  <InputGroupAddon addonType="prepend">
                    <InputGroupText style={{ background: 'transparent', border: 'none', paddingLeft: '1rem' }}>
                      <i className="fas fa-lock" style={{ color: '#4FD1C5' }} />
                    </InputGroupText>
                  </InputGroupAddon>
                  <Input 
                    placeholder="Password" 
                    onChange={onPasswordChange} 
                    type="password"
                    style={{ 
                      background: 'transparent', 
                      border: 'none',
                      fontSize: '0.95rem',
                      fontWeight: '500',
                    }}
                  />
                </InputGroup>
                {
                  passwordInvalid ? (
                    <Alert 
                      color="danger"
                      style={{
                        background: 'linear-gradient(135deg, rgba(252, 129, 129, 0.15), rgba(245, 101, 101, 0.05))',
                        border: '1px solid rgba(252, 129, 129, 0.3)',
                        borderRadius: '0.75rem',
                        color: '#FC8181',
                        fontWeight: '600',
                      }}
                    >
                      <i className="fas fa-exclamation-circle mr-2"></i>
                      Password is required
                    </Alert>)
                    : null
                }
              </FormGroup>
              <FormGroup className="d-flex align-items-center mb-4">
                <div className="custom-control custom-checkbox">
                  <Input
                    className="custom-control-input"
                    id="rememberMe"
                    type="checkbox"
                    onChange={onRememberMeChange}
                    checked={rememberMe}
                  />
                  <label
                    className="custom-control-label"
                    htmlFor="rememberMe"
                    style={{ 
                      color: '#525f7f',
                      fontWeight: '500',
                      fontSize: '0.9rem',
                      cursor: 'pointer',
                    }}
                  >
                    Remember me
                  </label>
                </div>
              </FormGroup>
              <FormGroup>
                {
                  loginAttempted ?
                    !loginSuccess ? (
                      <Alert 
                        color="danger"
                        style={{
                          background: 'linear-gradient(135deg, rgba(252, 129, 129, 0.15), rgba(245, 101, 101, 0.05))',
                          border: '1px solid rgba(252, 129, 129, 0.3)',
                          borderRadius: '0.75rem',
                          color: '#FC8181',
                          fontWeight: '600',
                        }}
                      >
                        <i className="fas fa-times-circle mr-2"></i>
                        {loginError}
                      </Alert>)
                      : <Alert 
                          color="success"
                          style={{
                            background: 'linear-gradient(135deg, rgba(79, 209, 197, 0.15), rgba(56, 178, 172, 0.05))',
                            border: '1px solid rgba(79, 209, 197, 0.3)',
                            borderRadius: '0.75rem',
                            color: '#4FD1C5',
                            fontWeight: '600',
                          }}
                        >
                          <i className="fas fa-check-circle mr-2"></i>
                          Login successful
                        </Alert>
                    : null
                }
              </FormGroup>
              <div className="text-center">
                <button 
                  type="submit" 
                  className="btn btn-lg btn-block"
                  disabled={isLoading}
                  style={{
                    background: isLoading ? 'linear-gradient(135deg, #aaa 0%, #888 100%)' : 'linear-gradient(135deg, #4FD1C5 0%, #63B3ED 100%)',
                    border: 'none',
                    borderRadius: '0.75rem',
                    padding: '0.875rem 2rem',
                    fontSize: '1rem',
                    fontWeight: '700',
                    color: '#fff',
                    boxShadow: '0 10px 30px rgba(79, 209, 197, 0.4)',
                    transition: 'all 0.3s ease',
                    cursor: isLoading ? 'not-allowed' : 'pointer',
                    position: 'relative',
                    overflow: 'hidden',
                  }}
                  onMouseEnter={(e) => {
                    if (!isLoading) {
                      e.currentTarget.style.transform = 'translateY(-2px)';
                      e.currentTarget.style.boxShadow = '0 15px 40px rgba(79, 209, 197, 0.5)';
                    }
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.transform = 'translateY(0)';
                    e.currentTarget.style.boxShadow = '0 10px 30px rgba(79, 209, 197, 0.4)';
                  }}
                >
                  {isLoading ? (
                    <>
                      <i className="fas fa-spinner fa-spin mr-2"></i>
                      Signing in...
                    </>
                  ) : (
                    <>
                      <i className="fas fa-sign-in-alt mr-2"></i>
                      Sign In
                    </>
                  )}
                </button>
              </div>
            </Form>
          </CardBody>
        </Card>
      </Col>
    </>
  );
}

export default Login;