import { useEffect } from "react";
import { Navigate } from "react-router-dom";
import { useDispatch } from 'react-redux';
import { logout } from "../store/actions/UserActions";


function Logout() {
  const dispatch = useDispatch();

  useEffect(() => {
    dispatch(logout());
  }, []);


  return <Navigate to="/auth/" />;
};

export default Logout;