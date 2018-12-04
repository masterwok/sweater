import React from 'react';
import SearchAppBar from "./SearchAppBar/SearchAppBar";
import ProgressBar from "./ProgressBar/ProgressBar";
import Search from "../../containers/Search/Search";


export default (props) => (
    <>
        <SearchAppBar
            title={'Sweater'}/>
        <ProgressBar/>
        <Search/>
    </>
);