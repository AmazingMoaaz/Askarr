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
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { Alert } from "reactstrap";
import { register } from "../store/actions/UserActions"

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


function Register() {
  const [isLoading, setIsLoading] = useState(false);
  const [username, setUsername] = useState("");
  const [usernameChanged, setUsernameChanged] = useState(false);
  const [password, setPassword] = useState("");
  const [passwordChanged, setPasswordChanged] = useState(false);
  const [passwordConfirmation, setPasswordConfirmation] = useState("");
  const [passwordConfirmationChanged, setPasswordConfirmationChanged] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);
  const [usernameInvalid, setUsernameInvalid] = useState(false);
  const [passwordInvalid, setPasswordInvalid] = useState(false);
  const [passwordConfirmationInvalid, setPasswordConfirmationInvalid] = useState(false);
  const [passwordsDoNotMatch, setPasswordsDoNotMatch] = useState(false);
  const [registrationAttempted, setRegistrationAttempted] = useState(false);
  const [registrationSuccess, setRegistrationSuccess] = useState(false);
  const [registrationError, setRegistrationError] = useState("");

  const history = useNavigate();
  const dispatch = useDispatch();


  useEffect(() => {
    if (usernameChanged) {
      triggerUsernameValidation();
    }
  }, [usernameChanged]);


  useEffect(() => {
    if (passwordChanged) {
      triggerPasswordValidation();
      triggerPasswordMatchValidation();
    }
  }, [passwordChanged]);

  useEffect(() => {
    if (passwordConfirmationChanged) {
      triggerPasswordConfirmationValidation();
      triggerPasswordMatchValidation();
    }
  }, [passwordConfirmationChanged]);



  const validateUsername = () => {
    return /\S/.test(username);
  };

  const validatePassword = () => {
    return /\S/.test(password);
  };

  const validatePasswordConfirmation = () => {
    return /\S/.test(passwordConfirmation);
  };

  const validatePasswordMatch = () => {
    return (!/\S/.test(password) || !/\S/.test(passwordConfirmation)) || (password === passwordConfirmation);
  };



  const onUsernameChange = event => {
    setUsername(event.target.value);
    setUsernameChanged(true);
  };

  const triggerUsernameValidation = () => {
    setUsernameInvalid(!validateUsername());
  };

  const onPasswordChange = event => {
    setPassword(event.target.value);
    setPasswordChanged(true);
  };

  const triggerPasswordValidation = () => {
    setPasswordInvalid(!validatePassword());
    setPasswordChanged(false);
  };

  const onPasswordConfirmationChange = event => {
    setPasswordConfirmation(event.target.value);
    setPasswordConfirmationChanged(true);
  };

  const triggerPasswordConfirmationValidation = () => {
    setPasswordConfirmationInvalid(!validatePasswordConfirmation());
    setPasswordConfirmationChanged(false);
  };

  const triggerPasswordMatchValidation = () => {
    setPasswordsDoNotMatch(!validatePasswordMatch());
  };

  const onRememberMeChange = event => {
    setRememberMe(!rememberMe);
  };

  const onRegister = e => {
    e.preventDefault();

    triggerUsernameValidation();
    triggerPasswordValidation();
    triggerPasswordConfirmationValidation();
    triggerPasswordMatchValidation();

    if (validateUsername()
      && validatePassword()
      && validatePasswordConfirmation()
      && validatePasswordMatch()) {

      setIsLoading(true);

      dispatch(register({
        username: username,
        password: password,
        passwordConfirmation: passwordConfirmation,
        rememberMe: rememberMe
      }))
        .then(data => {
          if (data.ok) {
            history('/admin/');
          }
          else {
            setIsLoading(false);
            let error = "An unknown error occurred while registrating.";

            if (typeof (data.error) === "string")
              error = data.error;

            setRegistrationAttempted(true);
            setRegistrationError(error);
            setRegistrationSuccess(false);
          }
        });
    }
  };



  return (
    <>
      <Col lg="6" md="8">
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
                  background: 'linear-gradient(135deg, #63B3ED 0%, #4FD1C5 100%)',
                  display: 'flex',
                  alignItems: 'center',
                  justifyContent: 'center',
                  boxShadow: '0 15px 40px rgba(99, 179, 237, 0.4)',
                  animation: 'pulse 2s ease-in-out infinite',
                }}
              >
                <i className="fas fa-user-plus" style={{ fontSize: '2rem', color: '#fff' }}></i>
              </div>
              <h2 
                className="mb-2"
                style={{
                  fontWeight: '800',
                  background: 'linear-gradient(135deg, #63B3ED 0%, #4FD1C5 100%)',
                  WebkitBackgroundClip: 'text',
                  WebkitTextFillColor: 'transparent',
                  backgroundClip: 'text',
                }}
              >
                Create Admin Account
              </h2>
              <p className="text-muted" style={{ fontSize: '0.95rem' }}>
                Set up your administrator credentials
              </p>
            </div>
            <Form role="form">
              <FormGroup className={usernameInvalid ? "has-danger" : username !== "" ? "has-success" : ""}>
                <InputGroup 
                  className="mb-3"
                  style={{
                    background: 'rgba(255, 255, 255, 0.8)',
                    borderRadius: '0.75rem',
                    border: usernameInvalid ? '2px solid #FC8181' : username !== "" ? '2px solid #4FD1C5' : '2px solid rgba(79, 209, 197, 0.3)',
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
                    name="username" 
                    value={username} 
                    onChange={onUsernameChange} 
                    placeholder="Username" 
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
              <FormGroup className={passwordInvalid ? "has-danger" : passwordChanged ? "has-success" : ""}>
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
                    value={password} 
                    onChange={onPasswordChange} 
                    placeholder="Password" 
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
              <FormGroup className={passwordConfirmationInvalid || passwordsDoNotMatch ? "has-danger" : passwordConfirmationChanged ? "has-success" : ""}>
                <InputGroup 
                  className="mb-3"
                  style={{
                    background: 'rgba(255, 255, 255, 0.8)',
                    borderRadius: '0.75rem',
                    border: (passwordConfirmationInvalid || passwordsDoNotMatch) ? '2px solid #FC8181' : passwordConfirmationChanged && passwordConfirmation !== "" ? '2px solid #4FD1C5' : '2px solid rgba(79, 209, 197, 0.3)',
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
                    value={passwordConfirmation} 
                    onChange={onPasswordConfirmationChange} 
                    placeholder="Confirm Password" 
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
                  passwordConfirmationInvalid ? (
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
                      Password confirmation is required
                    </Alert>)
                    : null
                }
                {
                  passwordsDoNotMatch ? (
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
                      Passwords do not match
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
                  registrationAttempted ?
                    !registrationSuccess ? (
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
                        {registrationError}
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
                          Registration successful
                        </Alert>
                    : null
                }
              </FormGroup>
              <div className="text-center">
                <button 
                  type="button" 
                  className="btn btn-lg btn-block"
                  onClick={onRegister} 
                  disabled={isLoading}
                  style={{
                    background: isLoading ? 'linear-gradient(135deg, #aaa 0%, #888 100%)' : 'linear-gradient(135deg, #63B3ED 0%, #4FD1C5 100%)',
                    border: 'none',
                    borderRadius: '0.75rem',
                    padding: '0.875rem 2rem',
                    fontSize: '1rem',
                    fontWeight: '700',
                    color: '#fff',
                    boxShadow: '0 10px 30px rgba(99, 179, 237, 0.4)',
                    transition: 'all 0.3s ease',
                    cursor: isLoading ? 'not-allowed' : 'pointer',
                    position: 'relative',
                    overflow: 'hidden',
                  }}
                  onMouseEnter={(e) => {
                    if (!isLoading) {
                      e.currentTarget.style.transform = 'translateY(-2px)';
                      e.currentTarget.style.boxShadow = '0 15px 40px rgba(99, 179, 237, 0.5)';
                    }
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.transform = 'translateY(0)';
                    e.currentTarget.style.boxShadow = '0 10px 30px rgba(99, 179, 237, 0.4)';
                  }}
                >
                  {isLoading ? (
                    <>
                      <i className="fas fa-spinner fa-spin mr-2"></i>
                      Creating account...
                    </>
                  ) : (
                    <>
                      <i className="fas fa-user-plus mr-2"></i>
                      Create Account
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

export default Register;