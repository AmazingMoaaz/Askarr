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


// reactstrap components
import { Container, Row, Col } from "reactstrap";
import { useEffect } from "react";
import { useDispatch, useSelector } from 'react-redux';
import { getVersionInfo } from "../../store/actions/VersionActions";

function AuthFooter() {
  const dispatch = useDispatch();
  const versionInfo = useSelector(state => state.version);

  useEffect(() => {
    dispatch(getVersionInfo());
  }, [dispatch]);

  return (
    <>
      <footer className="py-5">
        <Container>
          <Row className="align-items-center justify-content-center">
            <Col xl="6">
              <div 
                className="copyright d-flex justify-content-center text-center text-xl-left" 
                style={{
                  color: 'rgba(45, 55, 72, 0.7)',
                  fontWeight: '500',
                  fontSize: '0.875rem'
                }}
              >
                © {new Date().getFullYear()}{" "}
                <a 
                  href="https://github.com/AmazingMoaaz/Askarr" 
                  target="_blank" 
                  rel="noopener noreferrer"
                  style={{
                    color: '#4FD1C5',
                    textDecoration: 'none',
                    fontWeight: '600',
                    marginLeft: '0.25rem',
                    transition: 'all 0.3s ease'
                  }}
                  onMouseEnter={(e) => {
                    e.currentTarget.style.color = '#38B2AC';
                    e.currentTarget.style.textDecoration = 'underline';
                  }}
                  onMouseLeave={(e) => {
                    e.currentTarget.style.color = '#4FD1C5';
                    e.currentTarget.style.textDecoration = 'none';
                  }}
                >
                  Askarr
                </a>
                <span style={{ marginLeft: '0.25rem' }}>
                  (v{versionInfo?.currentVersion || '2.5.6'})
                </span>
              </div>
            </Col>
          </Row>
        </Container>
      </footer>
    </>
  );
}

export default AuthFooter;
