import React from 'react';
import IconButton from '@material-ui/core/IconButton';
import SettingsIcon from '@material-ui/icons/Settings';


const SettingsMenuItem = (props) => {
    return (
        <IconButton
            color="inherit">
            <SettingsIcon/>
        </IconButton>
    );
};

export default SettingsMenuItem;

