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
import { useEffect, useRef, useState } from "react";
import { Route, Routes, Navigate } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux';
// reactstrap components
import { Container } from "reactstrap";
// core components
import AdminFooter from "../components/Footers/AdminFooter.jsx";
import Sidebar from "../components/Sidebar/Sidebar.jsx";
import { validateLogin } from "../store/actions/UserActions";
import LoadingSpinner from "../components/Loaders/LoadingSpinner.jsx";

import routes from "../routes.js";
import AskarrLogo from "../assets/img/brand/logo.svg";


function Admin(props) {
  const [isLoading, setIsLoading] = useState(true);
  const mainContent = useRef(null);


  const reduxState = useSelector((state) => {
    return {
      isLoggedIn: state.user.isLoggedIn
    }
  });
  const dispatch = useDispatch();


  useEffect(() => {
    dispatch(validateLogin())
      .then(data => setIsLoading(false))
      .catch(error => {
        console.error("Login validation error:", error);
        setIsLoading(false);
      });
  }, [dispatch]);


  const getRoutes = routes => {
    return routes.map((prop, key) => {
      if (prop.layout === "/admin") {
        return (
          <Route
            path={prop.path}
            element={prop.component}
            key={key}
          />
        );
      } else {
        return null;
      }
    });
  };

  if (isLoading) {
    return <LoadingSpinner />;
  }

  return (
    <>
      <Sidebar
        {...props}
        routes={routes}
        logo={{
          innerLink: "/admin/",
          imgSrc: AskarrLogo,
          imgAlt: "Askarr Logo"
        }}
      />
      <div className="main-content" ref={mainContent}>
        <Routes>
          {
            reduxState.isLoggedIn
              ? getRoutes(routes)
              : null
          }
          {
            reduxState.isLoggedIn 
              ? <Route path="*" element={<Navigate to="/admin/chatclients" />} />
              : <Route path="*" element={<Navigate to="/auth/" />} />
          }
        </Routes>
        <Container fluid>
          <AdminFooter />
        </Container>
      </div>
    </>
  );
}

export default Admin;