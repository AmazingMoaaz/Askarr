import { useEffect, useRef } from "react";
import { Route, Routes, Navigate } from "react-router-dom";
import { useDispatch, useSelector } from 'react-redux';
import { Container } from "reactstrap";
import Sidebar from "../components/Sidebar/Sidebar.jsx";
import { validateLogin } from "../store/actions/UserActions";
import LoadingSpinner from "../components/Loaders/LoadingSpinner.jsx";
import routes from "../routes.js";
import AskarrLogo from "../assets/img/brand/logo.svg";

function Modern(props) {
  const mainContent = useRef(null);
  const dispatch = useDispatch();
  
  const { isLoggedIn, isLoading } = useSelector((state) => ({
    isLoggedIn: state.user.isLoggedIn,
    isLoading: state.user.isLoading
  }));

  useEffect(() => {
    dispatch(validateLogin());
  }, [dispatch]);

  useEffect(() => {
    document.documentElement.scrollTop = 0;
    document.scrollingElement.scrollTop = 0;
    if (mainContent.current) {
      mainContent.current.scrollTop = 0;
    }
  }, [props.location]);

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
    <div className="modern-layout">
      <Sidebar
        {...props}
        routes={routes}
        logo={{
          innerLink: "/admin/",
          imgSrc: AskarrLogo,
          imgAlt: "Askarr Logo"
        }}
        className="sidebar-modern"
      />
      <div className="main-content modern-main-content" ref={mainContent}>
        <div className="modern-content-wrapper">
          <Routes>
            {
              isLoggedIn
                ? getRoutes(routes)
                : null
            }
            {
              isLoggedIn 
                ? <Route path="*" element={<Navigate to="/admin/chatclients" />} />
                : <Route path="*" element={<Navigate to="/auth/" />} />
            }
          </Routes>
        </div>
      </div>
    </div>
  );
}

export default Modern; 